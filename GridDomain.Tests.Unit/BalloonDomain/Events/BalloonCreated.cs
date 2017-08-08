using System;
using GridDomain.EventSourcing;

namespace GridDomain.Tests.Unit.BalloonDomain.Events
{
    public class BalloonCreated : DomainEvent,
                                  IHaveProcessingHistory
    {
        public BalloonCreated(string value,
                              Guid sourceId,
                              DateTime? createdTime = default(DateTime?),
                              Guid processId = default(Guid)) : base(sourceId, processId, null, createdTime)
        {
            Value = value;
        }

        public string Value { get; }

        public ProcessedHistory History { get; } = new ProcessedHistory();
    }
}