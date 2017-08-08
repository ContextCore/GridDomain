using System;
using GridDomain.EventSourcing;

namespace GridDomain.Tests.Unit.EventsUpgrade.Domain.Events
{
    public class AggregateCreatedEvent : DomainEvent
    {
        public decimal Value;

        public AggregateCreatedEvent(decimal value,
                                     Guid sourceId,
                                     DateTime? createdTime = default(DateTime?),
                                     Guid processId = default(Guid)) : base(sourceId, processId: processId, createdTime: createdTime)
        {
            Value = value;
        }
    }
}