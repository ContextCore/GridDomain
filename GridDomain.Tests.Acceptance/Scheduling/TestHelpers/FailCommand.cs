using System;
using GridDomain.CQRS;

namespace GridDomain.Tests.Acceptance.Scheduling.TestHelpers
{
    public class FailCommand : Command
    {
        public FailCommand(TimeSpan timeout = default(TimeSpan)) : base(Guid.NewGuid())
        {
            Timeout = timeout;
        }

        public TimeSpan Timeout { get; }
    }
}