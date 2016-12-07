using System;
using GridDomain.CQRS;

namespace GridDomain.Tests.Acceptance.Scheduling.TestHelpers
{
    public class FailIfPlannedCommand : Command
    {
        public Guid AggregateId { get;}
        public TimeSpan Timeout { get; }

        public FailIfPlannedCommand(Guid aggregateId,TimeSpan timeout = default(TimeSpan))
        {
            AggregateId = aggregateId;
            Timeout = timeout;
        }
    }
}