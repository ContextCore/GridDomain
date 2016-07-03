using System;
using GridDomain.CQRS;

namespace GridDomain.Tests.FutureEvents.Infrastructure
{
    public class TestCommand : Command
    {
        public TestCommand(DateTime raiseTime, Guid aggregateId)
        {
            RaiseTime = raiseTime;
            AggregateId = aggregateId;
        }

        public Guid AggregateId { get; }
        public DateTime RaiseTime { get; }
    }
}