using System;
using GridDomain.EventSourcing;

namespace GridDomain.Tests.XUnit.BalloonDomain.Events
{
    public class BalloonTitleChanged : DomainEvent,
                                               IHaveProcessingHistory
    {
        public BalloonTitleChanged(string value,
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