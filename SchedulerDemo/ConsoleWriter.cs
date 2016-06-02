using System;
using Akka.Actor;

namespace SchedulerDemo
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
                    WriteHightlighted(DateTime.UtcNow.ToString("ss.fff"));
                }
                Console.ForegroundColor = ConsoleColor.DarkGreen;
                Console.Write($"       Received at: {DateTime.UtcNow.ToString("HH:mm:")}");
                WriteHightlighted(DateTime.UtcNow.ToString("ss.fff"));
                Console.WriteLine();
            });

            Receive<Failure>(msg =>
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