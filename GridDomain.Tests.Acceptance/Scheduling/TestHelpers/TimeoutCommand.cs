using System;
using GridDomain.Scheduling.Akka.Tasks;

namespace GridDomain.Tests.Acceptance.Scheduling.TestHelpers
{
    public class TimeoutCommand : ScheduledCommand
    {
        public TimeSpan Timeout { get; private set; }

        public TimeoutCommand(string taskId, string @group, TimeSpan timeout) : base(taskId, @group)
        {
            Timeout = timeout;
        }
    }
}