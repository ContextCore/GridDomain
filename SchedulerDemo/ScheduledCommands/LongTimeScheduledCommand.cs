using System;
using GridDomain.Scheduling.Akka.Tasks;

namespace SchedulerDemo.ScheduledCommands
{
    public class LongTimeScheduledCommand : ScheduledCommand
    {
        public TimeSpan Timeout { get; }

        public LongTimeScheduledCommand(string taskId, string group, TimeSpan timeout) : base(taskId, @group)
        {
            Timeout = timeout;
        }
    }
}