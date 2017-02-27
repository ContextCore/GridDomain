using System;
using GridDomain.CQRS;

namespace GridDomain.Tests.Acceptance.XUnit.Scheduling.TestHelpers
{
    public class FailCommand : Command
    {
        public TimeSpan Timeout { get; }

        public FailCommand(TimeSpan timeout = default(TimeSpan)):base(Guid.NewGuid())
        {
            Timeout = timeout;
        }
    }
}