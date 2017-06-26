using System;
using GridDomain.EventSourcing;

namespace GridDomain.Tests.XUnit.BalloonDomain.Events
{
    public class BalloonCreated : DomainEvent,
                                  IHaveProcessingHistory
    {
        public BalloonCreated(string value,
                              Guid sourceId,
                              DateTime? createdTime = default(DateTime?),
                              Guid sagaId = default(Guid)) : base(sourceId, sagaId, null, createdTime)
        {
            Value = value;
        }

        public string Value { get; }

        public ProcessedHistory History { get; } = new ProcessedHistory();
    }
}