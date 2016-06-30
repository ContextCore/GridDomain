using System;
using GridDomain.EventSourcing;

namespace GridDomain.Tests.SyncProjection.SampleDomain
{
    public class AggregateCreatedEvent : DomainEvent
    {
        public string Value;
        public ProcessedHistory History = new ProcessedHistory();
        public AggregateCreatedEvent(string value, Guid sourceId, DateTime? createdTime = default(DateTime?), Guid sagaId = default(Guid)) : base(sourceId, createdTime, sagaId)
        {
            Value = value;
        }
    }
}