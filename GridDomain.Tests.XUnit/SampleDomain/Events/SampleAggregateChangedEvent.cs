using System;
using GridDomain.EventSourcing;

namespace GridDomain.Tests.XUnit.SampleDomain.Events
{
    public class SampleAggregateChangedEvent : DomainEvent,
                                               IHaveProcessingHistory
    {
        public SampleAggregateChangedEvent(string value,
                                           Guid sourceId,
                                           DateTime? createdTime = null,
                                           Guid sagaId = new Guid()) : base(sourceId, sagaId: sagaId, createdTime: createdTime)
        {
            Value = value;
        }

        public string Value { get; }
        public ProcessedHistory History { get; } = new ProcessedHistory();
    }
}