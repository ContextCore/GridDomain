using System;
using GridDomain.CQRS;

namespace GridDomain.Tests.Acceptance.Scheduling.TestHelpers
{
    public class FailCommand : Command
    {
        public TimeSpan Timeout { get; }

        public FailCommand(TimeSpan timeout = default(TimeSpan))
        {
            Timeout = timeout;
        }
    }
}