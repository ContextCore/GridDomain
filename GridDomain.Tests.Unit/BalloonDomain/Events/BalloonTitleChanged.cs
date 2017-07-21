using System;
using GridDomain.EventSourcing;

namespace GridDomain.Tests.Unit.BalloonDomain.Events
{
    public class BalloonTitleChanged : DomainEvent,
                                               IHaveProcessingHistory
    {
        public BalloonTitleChanged(string value,
                                           Guid sourceId,
                                           DateTime? createdTime = null,
                                           Guid processId = new Guid()) : base(sourceId, processId: processId, createdTime: createdTime)
        {
            Value = value;
        }

        public string Value { get; }
        public ProcessedHistory History { get; } = new ProcessedHistory();
    }
}