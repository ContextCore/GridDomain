using System;
using System.Diagnostics;
using Akka.Actor;
using GridDomain.Common;
using GridDomain.CQRS.Messaging;
using GridDomain.Scheduling.Akka.Messages;
using SchedulerDemo.Events;
using SchedulerDemo.Messages;
using SchedulerDemo.ScheduledCommands;

namespace SchedulerDemo.Actors
{
    public class CommandManager : ReceiveActor
    {
        private readonly IPublisher _publisher;

        public CommandManager(IPublisher publisher)
        {
            _publisher = publisher;
            Receive<Scheduled>(x => _publisher.Publish(new WriteToConsole($"Task added, fire at : {x.NextExecution.ToLocalTime().ToString("HH:mm:")}", x.NextExecution.ToString("ss.fff"))));
            Receive<Unscheduled>(x => _publisher.Publish(new WriteToConsole($"Task {x.Key.Id} unscheduled")));
            Receive<AlreadyScheduled>(x => _publisher.Publish(new WriteToConsole($"Task {x.Key.Id} is already scheduled")));
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
                        _publisher.Publish(new WriteToConsole("unknown command, possible commands are: add, remove, fail, longtime"));
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
            var longTimeScheduledCommand = new LongTimeScheduledCommand("longtime", TimeSpan.FromSeconds(seconds));
            _publisher.Publish(new ScheduleCommand(longTimeScheduledCommand, new ScheduleKey(Guid.Empty, "long", "long"), CreateOptions(seconds)));
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
            var failScheduledCommand = new FailScheduledCommand();
            _publisher.Publish(new ScheduleCommand(failScheduledCommand, new ScheduleKey(Guid.Empty, "fail", "fail"), CreateOptions(seconds)));
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
            var schedule = new ScheduleCommand(new WriteToConsoleScheduledCommand(text), new ScheduleKey(Guid.Empty, text, text), CreateOptions(seconds));
            _publisher.Publish(schedule);
        }

        private void Unschedule(string[] parts)
        {
            if (parts.Length != 2)
            {
                _publisher.Publish(new WriteToConsole("wrong command format"));
                return;
            }

            var text = parts[1];
            _publisher.Publish(new Unschedule(new ScheduleKey(Guid.Empty, text, text)));
        }

        private static ExecutionOptions CreateOptions(int seconds)
        {
            return new ExecutionOptions<ScheduledCommandSuccessfullyProcessed>(BusinessDateTime.UtcNow.AddSeconds(seconds), ExecutionTimeout);
        }

        private static TimeSpan ExecutionTimeout
        {
            get
            {
                if (Debugger.IsAttached)
                {
                    return TimeSpan.FromMinutes(1);
                }
                return TimeSpan.FromSeconds(3);
            }
        }
    }
}