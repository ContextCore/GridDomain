using System;
using System.Diagnostics;
using Akka.Actor;
using GridDomain.Scheduling.Akka.Messages;
using SchedulerDemo.Messages;
using SchedulerDemo.ScheduledRequests;

namespace SchedulerDemo.Actors
{
    public class CommandManager : ReceiveActor
    {
        public CommandManager()
        {
            Receive<Scheduled>(x => ActorReferences.Writer.Tell(new WriteToConsole($"Task added, fire at : {x.NextExecution.ToString("HH:mm:")}", x.NextExecution.ToString("ss.fff"))));
            Receive<Unscheduled>(x => ActorReferences.Writer.Tell(new WriteToConsole($"Task {x.TaskId} unscheduled")));
            Receive<AlreadyScheduled>(x => ActorReferences.Writer.Tell(new WriteToConsole($"Task {x.TaskId} is already scheduled")));
            Receive<Failure>(x => ActorReferences.Writer.Forward(x));
            Receive<ProcessCommand>(x =>
            {
                var parts = x.Command.Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                var command = parts[0];
                switch (command)
                {
                    case "add":
                        Schedule(parts);
                        break;
                    case "remove":
                        Unschedule(parts);
                        break;
                    default:
                        ActorReferences.Writer.Tell(new WriteToConsole("unknown command"));
                        break;
                }
            });
        }

        private void Schedule(string[] parts)
        {
            if (parts.Length != 3)
            {
                ActorReferences.Writer.Tell(new WriteToConsole("wrong command format"));
                return;
            }

            var text = parts[1];
            var secondsToWaitString = parts[2];
            int seconds;
            if (!int.TryParse(secondsToWaitString, out seconds))
            {
                ActorReferences.Writer.Tell(new WriteToConsole("wrong command format"));
            }
            var taskId = text;// + Guid.NewGuid().ToString().Substring(0, 8);

            ActorReferences.Scheduler.Tell(new Schedule(new WriteToConsoleScheduledMessage(taskId, taskId), DateTime.UtcNow.AddSeconds(seconds), ExecutionTimeout()));
        }

        private void Unschedule(string[] parts)
        {
            if (parts.Length != 2)
            {
                ActorReferences.Writer.Tell(new WriteToConsole("wrong command format"));
                return;
            }

            var text = parts[1];
            var taskId = text;// + Guid.NewGuid().ToString().Substring(0, 8);
            ActorReferences.Scheduler.Tell(new Unschedule(taskId, taskId));
        }

        private static TimeSpan ExecutionTimeout()
        {
            if (Debugger.IsAttached)
            {
                return TimeSpan.FromMinutes(1);
            }
            return TimeSpan.FromSeconds(3);
        }
    }
}