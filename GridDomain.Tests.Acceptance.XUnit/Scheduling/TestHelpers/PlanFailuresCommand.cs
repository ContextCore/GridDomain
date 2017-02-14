using System;
using GridDomain.CQRS;

namespace GridDomain.Tests.Acceptance.XUnit.Scheduling.TestHelpers
{
    public class PlanFailuresCommand : Command
    {
        public Guid AggregateId { get; }
        public int FailsNum { get; }

        public PlanFailuresCommand(Guid aggregateId, int failsNum)
        {
            AggregateId = aggregateId;
            FailsNum = failsNum;
        }
    }
}