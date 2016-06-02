using System;
using System.Collections.Generic;
using Akka.Actor;
using GridDomain.Scheduling.Integration;
using GridDomain.Scheduling.Quartz;
using GridDomain.Scheduling.Quartz.Logging;

namespace SchedulerDemo
{
    class Program
    {
        public static ActorSystem Sys;

        static void Main()
        {
            Sys = ActorSystem.Create("Sys");
            ActorReferences.Reader = Sys.ActorOf(Props.Create<ConsoleReader>());
            ActorReferences.Writer = Sys.ActorOf(Props.Create<ConsoleWriter>());
            var schedulerFactory = new SchedulerFactory(new QuartzConfig(), new LoggingSchedulerListener(), new LoggingJobListener());
            ActorReferences.Scheduler = Sys.ActorOf(Props.Create(() => new SchedulerActor(schedulerFactory.Create())));
            ActorReferences.Handler = Sys.ActorOf(Props.Create<WriteToConsoleScheduledHandler>());
            ActorReferences.CommandManager = Sys.ActorOf(Props.Create<CommandManager>());
            var taskRouter = new TaskRouter(new Dictionary<Type, IActorRef>
            {
                {typeof(WriteToConsoleRequest), ActorReferences.Handler}
            });
            TaskRouterFactory.Init(taskRouter);
            ActorReferences.Writer.Tell(new WriteToConsole("started"));
            ActorReferences.Reader.Tell(new StartReadFromConsole());

            Sys.WhenTerminated.Wait();
        }
    }
}
