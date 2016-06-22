using Akka.Actor;
using Akka.DI.Core;
using Akka.DI.Unity;
using GridDomain.CQRS.Messaging;
using GridDomain.CQRS.Messaging.Akka;
using GridDomain.CQRS.Messaging.MessageRouting;
using GridDomain.Node;
using GridDomain.Node.Actors;
using GridDomain.Node.Configuration;
using GridDomain.Scheduling;
using GridDomain.Scheduling.Akka.Messages;
using GridDomain.Scheduling.Integration;
using GridDomain.Scheduling.Quartz.Logging;
using GridDomain.Scheduling.WebUI;
using Microsoft.Practices.Unity;
using SchedulerDemo.Actors;
using SchedulerDemo.AgregateHandler;
using SchedulerDemo.Configuration;
using SchedulerDemo.Messages;
using CompositionRoot = GridDomain.Node.CompositionRoot;

namespace SchedulerDemo
{
    class Program
    {
        public static ActorSystem Sys;

        static void Main()
        {
            Sys = ActorSystemFactory.CreateActorSystem(new LocalAkkaConfiguration(AkkaConfiguration.LogVerbosity.Error));
            var container = Container.Current.CreateChildContainer();
            CompositionRoot.Init(container, Sys, new LocalDbConfiguration(), TransportMode.Standalone);
            RegisterAppSpecificTypes(container);
            Sys.AddDependencyResolver(new UnityDependencyResolver(container, Sys));
            var routing = new ConsoleAggregateRouting();

            var webUiWrapper = container.Resolve<IWebUiWrapper>();
            var gridNode = new GridDomainNode(container, routing, TransportMode.Standalone, Sys);
            gridNode.Start(new LocalDbConfiguration());
            using (webUiWrapper.Start())
            {
                var reader = Sys.ActorOf(Sys.DI().Props<ConsoleReader>());
                var writer = Sys.ActorOf(Sys.DI().Props<ConsoleWriter>());
                var scheduler = Sys.ActorOf(Sys.DI().Props<SchedulingActor>());
                var commandManager = Sys.ActorOf(Sys.DI().Props<CommandManager>());
                IActorSubscriber subsriber = container.Resolve<IActorSubscriber>();
                //subsriber.Subscribe<WriteToConsoleScheduledCommand>(handler);
                subsriber.Subscribe<ProcessCommand>(commandManager);
                subsriber.Subscribe<StartReadFromConsole>(reader);
                subsriber.Subscribe<WriteToConsole>(writer);
                subsriber.Subscribe<WriteErrorToConsole>(writer);
                subsriber.Subscribe<ScheduleCommand>(scheduler);
                subsriber.Subscribe<Unschedule>(scheduler);
                var publisher = container.Resolve<IPublisher>();
                publisher.Publish(new WriteToConsole("started"));
                publisher.Publish(new StartReadFromConsole());

                gridNode.System.WhenTerminated.Wait();
            }
        }

        private static void RegisterAppSpecificTypes(IUnityContainer container)
        {
            container.RegisterType<AggregateActor<ConsoleAggregate>>();
            container.RegisterType<AggregateHubActor<ConsoleAggregate>>();
            container.RegisterType<ICommandAggregateLocator<ConsoleAggregate>, ConsoleAggregateCommadHandler>();
            container.RegisterType<IAggregateCommandsHandler<ConsoleAggregate>, ConsoleAggregateCommadHandler>();
            container.RegisterType<IQuartzLogger, DemoLogger>();
            container.RegisterType<ConsoleReader>();
            container.RegisterType<ConsoleWriter>();
            container.RegisterType<CommandManager>();
        }
    }
}
