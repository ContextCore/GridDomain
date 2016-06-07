using System;
using System.Diagnostics;
using Akka.Actor;
using GridDomain.CQRS.Messaging;
using GridDomain.Scheduling.Akka.Messages;
using SchedulerDemo.Messages;
using SchedulerDemo.ScheduledMessages;

namespace SchedulerDemo.Actors
{
    public class CommandManager : ReceiveActor
    {
        private readonly IPublisher _publisher;

        public CommandManager(IPublisher publisher)
        {
            _publisher = publisher;
            Receive<Scheduled>(x => _publisher.Publish(new WriteToConsole($"Task added, fire at : {x.NextExecution.ToLocalTime().ToString("HH:mm:")}", x.NextExecution.ToString("ss.fff"))));
            Receive<Unscheduled>(x => _publisher.Publish(new WriteToConsole($"Task {x.TaskId} unscheduled")));
            Receive<AlreadyScheduled>(x => _publisher.Publish(new WriteToConsole($"Task {x.TaskId} is already scheduled")));
            Receive<Failure>(x => _publisher.Publish(new WriteErrorToConsole(x.Exception)));
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
                    case "fail":
                        ScheduleFailure(parts);
                        break;
                    case "longtime":
                        ScheduleLongTime(parts);
                        break;
                    default:
                        _publisher.Publish(new WriteToConsole("unknown command"));
                        break;
                }
            });
        }

        private void ScheduleLongTime(string[] parts)
        {
            if (parts.Length != 2)
            {
                _publisher.Publish(new WriteToConsole("wrong command format"));
                return;
            }
            var secondsToWaitString = parts[1];
            int seconds;
            if (!int.TryParse(secondsToWaitString, out seconds))
            {
                _publisher.Publish(new WriteToConsole("wrong command format"));
            }
            _publisher.Publish(new Schedule(new LongTimeScheduledMessage("long", "long", seconds), DateTime.UtcNow, TimeSpan.FromMinutes(1)));
        }

        private void ScheduleFailure(string[] parts)
        {
            if (parts.Length != 2)
            {
                _publisher.Publish(new WriteToConsole("wrong command format"));
                return;
            }
            var secondsToWaitString = parts[1];
            int seconds;
            if (!int.TryParse(secondsToWaitString, out seconds))
            {
                _publisher.Publish(new WriteToConsole("wrong command format"));
            }
            _publisher.Publish(new Schedule(new FailScheduledMessage("fail", "fail"), DateTime.UtcNow.AddSeconds(seconds), TimeSpan.FromSeconds(4)));
        }

        private void Schedule(string[] parts)
        {
            if (parts.Length != 3)
            {
                _publisher.Publish(new WriteToConsole("wrong command format"));
                return;
            }

            var text = parts[1];
            var secondsToWaitString = parts[2];
            int seconds;
            if (!int.TryParse(secondsToWaitString, out seconds))
            {
                _publisher.Publish(new WriteToConsole("wrong command format"));
            }
            var taskId = text;// + Guid.NewGuid().ToString().Substring(0, 8);

            _publisher.Publish(new Schedule(new WriteToConsoleScheduledMessage(taskId, taskId), DateTime.UtcNow.AddSeconds(seconds), ExecutionTimeout()));
        }

        private void Unschedule(string[] parts)
        {
            if (parts.Length != 2)
            {
                _publisher.Publish(new WriteToConsole("wrong command format"));
                return;
            }

            var text = parts[1];
            var taskId = text;// + Guid.NewGuid().ToString().Substring(0, 8);
            _publisher.Publish(new Unschedule(taskId, taskId));
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