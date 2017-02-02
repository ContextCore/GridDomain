using System;
using GridDomain.EventSourcing;

namespace GridDomain.Tests.XUnit.SampleDomain.Events
{
    public class SampleAggregateCreatedEvent : DomainEvent, IHaveProcessingHistory
    {
        public string Value { get; }
        
        public ProcessedHistory History { get; } = new ProcessedHistory();
        public SampleAggregateCreatedEvent(string value, Guid sourceId, DateTime? createdTime = default(DateTime?), Guid sagaId = default(Guid)) : base(sourceId, createdTime, sagaId)
        {
            Value = value;
        }
    }
}