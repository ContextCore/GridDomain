using Akka.Actor;
using Akka.DI.Core;
using Akka.DI.Unity;
using GridDomain.CQRS.Messaging;
using GridDomain.Node;
using GridDomain.Scheduling;
using GridDomain.Scheduling.Akka.Messages;
using GridDomain.Scheduling.Integration;
using GridDomain.Scheduling.Quartz.Logging;
using GridDomain.Scheduling.WebUI;
using Microsoft.Practices.Unity;
using SchedulerDemo.Actors;
using SchedulerDemo.Handlers;
using SchedulerDemo.Messages;
using SchedulerDemo.ScheduledMessages;
using CompositionRoot = GridDomain.Scheduling.CompositionRoot;

namespace SchedulerDemo
{
    class Program
    {
        public static ActorSystem Sys;

        static void Main()
        {
            Sys = ActorSystem.Create("Sys");
            var container = new CompositionRoot().Compose(Container.Current.CreateChildContainer(), Sys);

            RegisterAppSpecificTypes(container);
            Sys.AddDependencyResolver(new UnityDependencyResolver(container, Sys));
            using (container.Resolve<IWebUiWrapper>().Start())
            {
                var reader = Sys.ActorOf(Sys.DI().Props<ConsoleReader>());
                var writer = Sys.ActorOf(Sys.DI().Props<ConsoleWriter>());
                var scheduler = Sys.ActorOf(Sys.DI().Props<SchedulingActor>());
                var handler = Sys.ActorOf(Sys.DI().Props<WriteToConsoleScheduledHandler>());
                var failHandler = Sys.ActorOf(Sys.DI().Props<FailingScheduledHandler>());
                var longTimeHandler = Sys.ActorOf(Sys.DI().Props<LongTimeScheduledHandler>());
                var commandManager = Sys.ActorOf(Sys.DI().Props<CommandManager>());
                IActorSubscriber subsriber = container.Resolve<IActorSubscriber>();
                subsriber.Subscribe<WriteToConsoleScheduledCommand>(handler);
                subsriber.Subscribe<ProcessCommand>(commandManager);
                subsriber.Subscribe<StartReadFromConsole>(reader);
                subsriber.Subscribe<WriteToConsole>(writer);
                subsriber.Subscribe<WriteErrorToConsole>(writer);
                subsriber.Subscribe<Schedule>(scheduler);
                subsriber.Subscribe<Unschedule>(scheduler);
                subsriber.Subscribe<FailScheduledCommand>(failHandler);
                subsriber.Subscribe<LongTimeScheduledCommand>(longTimeHandler);
                var publisher = container.Resolve<IPublisher>();
                publisher.Publish(new WriteToConsole("started"));
                publisher.Publish(new StartReadFromConsole());

                Sys.WhenTerminated.Wait();
            }
        }

        private static void RegisterAppSpecificTypes(IUnityContainer container)
        {
            container.RegisterType<IQuartzLogger, DemoLogger>();
            container.RegisterType<ConsoleReader>();
            container.RegisterType<ConsoleWriter>();
            container.RegisterType<WriteToConsoleScheduledHandler>();
            container.RegisterType<CommandManager>();
        }
    }
}
