using System;
using System.Diagnostics;
using Akka.Actor;
using GridDomain.Scheduling.Akka.Messages;

namespace SchedulerDemo
{
    public class ProcessCommand
    {
        public string Command { get; }

        public ProcessCommand(string command)
        {
            Command = command;
        }
    }

    public class CommandManager : ReceiveActor
    {
        public CommandManager()
        {
            Receive<TaskAdded>(x => ActorReferences.Writer.Tell(new WriteToConsole($"Task added, fire at : {x.NextExecution.ToString("HH:mm:")}", x.NextExecution.ToString("ss.fff"))));
            Receive<Failure>(x => ActorReferences.Writer.Forward(x));
            Receive<ProcessCommand>(x =>
            {
                var parts = x.Command.Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                var command = parts[0];
                switch (command)
                {
                    case "s":
                        Schedule(parts);
                        break;
                }
            });
        }

        private void Schedule(string[] parts)
        {
            if (parts.Length != 3)
            {
                ActorReferences.Writer.Tell(new WriteToConsole("wrong format command"));
                return;
            }

            var text = parts[1];
            var secondsToWaitString = parts[2];
            int seconds;
            if (!int.TryParse(secondsToWaitString, out seconds))
            {
                ActorReferences.Writer.Tell(new WriteToConsole("wrong format command"));
            }
            var taskId = text;// + Guid.NewGuid().ToString().Substring(0, 8);

            ActorReferences.Scheduler.Tell(new AddTask(new WriteToConsoleRequest(taskId), DateTime.UtcNow.AddSeconds(seconds), ExecutionTimeout()));
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

    public class ConsoleReader : ReceiveActor
    {
        public ConsoleReader()
        {
            Receive<StartReadFromConsole>(msg =>
            {
                var read = Console.ReadLine();
                if (!string.IsNullOrEmpty(read) && read == "close")
                {
                    Context.System.Terminate();
                }
                if (string.IsNullOrEmpty(read))
                {
                    ActorReferences.Writer.Tell(new WriteToConsole("wrong input"));
                }
                else
                {
                    if (read.StartsWith("command "))
                    {
                        ActorReferences.CommandManager.Tell(new ProcessCommand(read.Substring(7)));
                    }
                }
                Self.Tell(new StartReadFromConsole());
            });
        }
    }
}