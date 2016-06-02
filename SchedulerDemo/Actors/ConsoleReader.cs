using System;
using Akka.Actor;
using SchedulerDemo.Messages;

namespace SchedulerDemo.Actors
{
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