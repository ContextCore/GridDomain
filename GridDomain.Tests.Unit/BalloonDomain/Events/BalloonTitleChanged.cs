using System;
using GridDomain.Common;
using GridDomain.CQRS;
using GridDomain.EventSourcing;

namespace GridDomain.Tests.Unit.BalloonDomain.Events
{
    public class BalloonTitleChanged : DomainEvent,
                                       IHaveProcessingHistory,
                                       IFor<Balloon>
    
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