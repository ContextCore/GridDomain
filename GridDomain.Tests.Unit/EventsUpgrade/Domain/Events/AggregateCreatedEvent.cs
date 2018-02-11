using System;
using GridDomain.EventSourcing;

namespace GridDomain.Tests.Unit.EventsUpgrade.Domain.Events
{
    public class AggregateCreatedEvent : DomainEvent
    {
        public decimal Value;

        public AggregateCreatedEvent(decimal value,
                                     string sourceId,
                                     DateTime? createdTime = default(DateTime?),
                                     string processId = null) : base(sourceId, processId: processId, createdTime: createdTime)
        {
            Value = value;
        }
    }
}