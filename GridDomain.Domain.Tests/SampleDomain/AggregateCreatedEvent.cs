using System;
using GridDomain.EventSourcing;

namespace GridDomain.Tests.SampleDomain
{
    public class AggregateCreatedEvent : DomainEvent, IHaveProcessingHistory
    {
        public string Value;
        public ProcessedHistory History { get; } = new ProcessedHistory();
        public AggregateCreatedEvent(string value, Guid sourceId, DateTime? createdTime = default(DateTime?), Guid sagaId = default(Guid)) : base(sourceId, createdTime, sagaId)
        {
            Value = value;
        }
    }
}