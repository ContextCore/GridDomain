using System;
using GridDomain.CQRS;
using GridDomain.ProcessManagers;

namespace GridDomain.Tests.Acceptance.Scheduling.TestHelpers
{
    public class FailCommand : Command<TestAggregate>
    {
        public FailCommand(TimeSpan timeout = default(TimeSpan)) : base(Guid.NewGuid().ToString())
        {
            Timeout = timeout;
        }

        public TimeSpan Timeout { get; }
    }
}