using System;
using GridDomain.EventSourcing;

namespace GridDomain.Tests.Unit.BalloonDomain.Events
{
    public class BalloonCreated : DomainEvent<Balloon>,
                                  IHaveProcessingHistory
    {
        public BalloonCreated(string value,
                              string sourceId,
                              DateTime? createdTime = default(DateTime?),
                              string processId = null) : base(sourceId, processId, null, createdTime)
        {
            Value = value;
        }

        public string Value { get; }

        public ProcessedHistory History { get; } = new ProcessedHistory();
    }
}