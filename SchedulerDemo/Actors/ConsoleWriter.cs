using System;
using Akka.Actor;
using GridDomain.Common;
using SchedulerDemo.Messages;

namespace SchedulerDemo.Actors
{
    public class ConsoleWriter : ReceiveActor
    {
        public ConsoleWriter()
        {
            Receive<WriteToConsole>(msg =>
            {
                Console.Write(msg.Text);
                if (msg.PartToHighlight != null)
                {
                    WriteHightlighted(DateTimeFacade.Now.ToString("ss.fff"));
                }
                Console.ForegroundColor = ConsoleColor.DarkGreen;
                Console.SetCursorPosition(50, Console.CursorTop);
                Console.Write($"       Received at: {DateTimeFacade.Now.ToString("HH:mm:")}");
                WriteHightlighted(DateTimeFacade.Now.ToString("ss.fff"));
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