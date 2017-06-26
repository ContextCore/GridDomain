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
                                     Guid sagaId = default(Guid)) : base(sourceId, sagaId: sagaId, createdTime: createdTime)
        {
            Value = value;
        }
    }
}