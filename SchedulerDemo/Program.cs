using System;
using System.Collections.Generic;
using Akka.Actor;
using Akka.DI.Core;
using Akka.DI.Unity;
using GridDomain.CQRS.Messaging;
using GridDomain.Node;
using GridDomain.Scheduling;
using GridDomain.Scheduling.Akka.Messages;
using GridDomain.Scheduling.Integration;
using GridDomain.Scheduling.Quartz;
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
            var container = new CompositionRoot().Compose(Container.Current, Sys);

            RegisterAppSpecificTypes(container);
            Sys.AddDependencyResolver(new UnityDependencyResolver(container, Sys));
            using (container.Resolve<IWebUiWrapper>().Start())
            {
                var reader = Sys.ActorOf(Sys.DI().Props<ConsoleReader>());
                var writer = Sys.ActorOf(Sys.DI().Props<ConsoleWriter>());
                var scheduler = Sys.ActorOf(Sys.DI().Props<SchedulingActor>());
                var handler = Sys.ActorOf(Sys.DI().Props<WriteToConsoleScheduledHandler>());
                var commandManager = Sys.ActorOf(Sys.DI().Props<CommandManager>());
                IActorSubscriber subsriber = container.Resolve<IActorSubscriber>();
                subsriber.Subscribe(typeof(WriteToConsoleScheduledMessage), handler);
                subsriber.Subscribe(typeof(ProcessCommand), commandManager);
                subsriber.Subscribe(typeof(StartReadFromConsole), reader);
                subsriber.Subscribe(typeof(WriteToConsole), writer);
                subsriber.Subscribe(typeof(WriteError), writer);
                subsriber.Subscribe(typeof(Schedule), scheduler);
                subsriber.Subscribe(typeof(Unschedule), scheduler);
                var publisher = container.Resolve<IPublisher>();
                publisher.Publish(new WriteToConsole("started"));
                publisher.Publish(new StartReadFromConsole());

                Sys.WhenTerminated.Wait();
            }
        }

        private static void RegisterAppSpecificTypes(UnityContainer container)
        {
            container.RegisterType<ConsoleReader>();
            container.RegisterType<ConsoleWriter>();
            container.RegisterType<WriteToConsoleScheduledHandler>();
            container.RegisterType<CommandManager>();
        }
    }
}
