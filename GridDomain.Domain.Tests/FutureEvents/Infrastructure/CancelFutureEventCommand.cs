using System;
using GridDomain.CQRS;

namespace GridDomain.Tests.FutureEvents.Infrastructure
{
    public class CancelFutureEventCommand : Command
    {
        public CancelFutureEventCommand(Guid aggregateId, string value)
        {
            AggregateId = aggregateId;
            Value = value;
        }
        public Guid AggregateId { get; }
        public string Value { get; }
    }
}