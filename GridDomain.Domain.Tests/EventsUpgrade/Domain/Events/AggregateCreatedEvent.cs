using System;
using GridDomain.EventSourcing;

namespace GridDomain.Tests.EventsUpgrade.Domain.Events
{
    public class AggregateCreatedEvent : DomainEvent
    {
        public decimal Value;
        public AggregateCreatedEvent(decimal value, Guid sourceId, DateTime? createdTime = default(DateTime?), Guid sagaId = default(Guid)) : base(sourceId, createdTime, sagaId)
        {
            Value = value;
        }
    }
}