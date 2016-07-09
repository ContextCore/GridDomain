using System;
using GridDomain.EventSourcing;

namespace GridDomain.Tests.SampleDomain
{
    public class AggregateChangedEvent : DomainEvent, IHaveProcessingHistory
    {
        public ProcessedHistory History { get; } = new ProcessedHistory();
        public  string Value;
        public AggregateChangedEvent(string value,Guid sourceId, DateTime? createdTime = null, Guid sagaId = new Guid()) : base(sourceId, createdTime, sagaId)
        {
            Value = value;
        }
    }
}