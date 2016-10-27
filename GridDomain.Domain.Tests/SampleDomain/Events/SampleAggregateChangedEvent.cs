using System;
using GridDomain.EventSourcing;

namespace GridDomain.Tests.SampleDomain.Events
{
    public class SampleAggregateChangedEvent : DomainEvent, IHaveProcessingHistory
    {
        public ProcessedHistory History { get; } = new ProcessedHistory();
        public string Value { get; }
        
        public SampleAggregateChangedEvent(string value,Guid sourceId, DateTime? createdTime = null, Guid sagaId = new Guid()) : base(sourceId, createdTime, sagaId)
        {
            Value = value;
        }
    }
}