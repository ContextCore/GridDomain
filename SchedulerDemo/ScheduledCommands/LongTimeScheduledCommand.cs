using System;
using GridDomain.CQRS;

namespace SchedulerDemo.ScheduledCommands
{
    public class LongTimeScheduledCommand : Command
    {
        public string Text { get; }
        public TimeSpan Timeout { get; }

        public LongTimeScheduledCommand(string text, TimeSpan timeout)
        {
            Text = text;
            Timeout = timeout;
        }
    }
}