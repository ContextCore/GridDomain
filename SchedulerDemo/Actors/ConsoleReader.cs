using System;
using Akka.Actor;
using GridDomain.CQRS.Messaging;
using SchedulerDemo.Messages;

namespace SchedulerDemo.Actors
{
    public class ConsoleReader : ReceiveActor
    {
        private readonly IPublisher _publisher;

        public ConsoleReader(IPublisher publisher)
        {
            _publisher = publisher;
            Receive<StartReadFromConsole>(msg =>
            {
                var read = Console.ReadLine();
                if (!string.IsNullOrEmpty(read) && read == "close")
                {
                    Context.System.Terminate();
                }
                if (string.IsNullOrEmpty(read))
                {
                    _publisher.Publish(new WriteToConsole("wrong input"));
                }
                else
                {
                    _publisher.Publish(new ProcessCommand(read));
                }
                Self.Tell(new StartReadFromConsole());
            });
        }
    }
}