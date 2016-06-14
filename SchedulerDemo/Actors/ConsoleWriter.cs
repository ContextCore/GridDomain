using System;
using Akka.Actor;
using CommonDomain.Core;
using GridDomain.CQRS.Messaging.MessageRouting;
using SchedulerDemo.Messages;

namespace SchedulerDemo.Actors
{
    public class ConsoleWriterCommandHandlerAggregate : AggregateCommandsHandler<ConsoleWriterAggregate>
    {
        public ConsoleWriterCommandHandlerAggregate()
        {
            Map<WriteToConsole>(c => c.Id, (c, a) => a.WriteToConsole(c.Text));
        }
    }

    public class ConsoleWriterAggregate : AggregateBase
    {
        public void WriteToConsole(string text)
        {
            Console.WriteLine(text);
        }
    }

    public class ConsoleWriter : ReceiveActor
    {
        public ConsoleWriter()
        {
            Receive<WriteToConsole>(msg =>
            {
                Console.Write(msg.Text);
                if (msg.PartToHighlight != null)
                {
                    WriteHightlighted(DateTime.Now.ToString("ss.fff"));
                }
                Console.ForegroundColor = ConsoleColor.DarkGreen;
                Console.SetCursorPosition(50, Console.CursorTop);
                Console.Write($"       Received at: {DateTime.Now.ToString("HH:mm:")}");
                WriteHightlighted(DateTime.Now.ToString("ss.fff"));
                Console.SetCursorPosition(0, Console.CursorTop);
                Console.WriteLine();
            });

            Receive<WriteErrorToConsole>(msg =>
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(msg.Exception.ToString());
                Console.ForegroundColor = ConsoleColor.White;
            });
        }

        private static void WriteHightlighted(string highlighted)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write(highlighted);
            Console.ForegroundColor = ConsoleColor.White;
        }
    }
}