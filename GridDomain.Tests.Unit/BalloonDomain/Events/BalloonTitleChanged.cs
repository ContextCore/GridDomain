using System;
using GridDomain.EventSourcing;

namespace GridDomain.Tests.Unit.BalloonDomain.Events
{
    public class BalloonTitleChanged : DomainEvent,
                                               IHaveProcessingHistory
    {
        public BalloonTitleChanged(string value,
                                   string sourceId,
                                           DateTime? createdTime = null,
                                   string processId = null) : base(sourceId, processId: processId, createdTime: createdTime)
        {
            Value = value;
        }

        public string Value { get; }
        public ProcessedHistory History { get; } = new ProcessedHistory();
    }
}