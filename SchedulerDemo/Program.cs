using System;
using System.Collections.Generic;
using Akka.Actor;
using Akka.DI.Core;
using Akka.DI.Unity;
using GridDomain.Node;
using GridDomain.Scheduling;
using GridDomain.Scheduling.Integration;
using GridDomain.Scheduling.Quartz;
using GridDomain.Scheduling.Quartz.Logging;
using GridDomain.Scheduling.WebUI;
using Microsoft.Practices.Unity;
using SchedulerDemo.Actors;
using SchedulerDemo.Handlers;
using SchedulerDemo.Messages;
using SchedulerDemo.ScheduledRequests;
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
                ActorReferences.Reader = Sys.ActorOf(Sys.DI().Props<ConsoleReader>());
                ActorReferences.Writer = Sys.ActorOf(Sys.DI().Props<ConsoleWriter>());
                ActorReferences.Scheduler = Sys.ActorOf(Sys.DI().Props<SchedulingActor>());
                ActorReferences.Handler = Sys.ActorOf(Sys.DI().Props<WriteToConsoleScheduledHandler>());
                ActorReferences.CommandManager = Sys.ActorOf(Sys.DI().Props<CommandManager>());
                var subsriber = Container.Current.Resolve<IActorSubscriber>();
                subsriber.Subscribe(typeof(WriteToConsoleScheduledMessage), ActorReferences.Handler);
                ActorReferences.Writer.Tell(new WriteToConsole("started"));
                ActorReferences.Reader.Tell(new StartReadFromConsole());

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
