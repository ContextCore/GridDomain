using System;
using GridDomain.EventSourcing;

namespace GridDomain.Scheduling.FutureEvents
{
    public class FutureEventScheduledEvent : DomainEvent, IFutureDomainEvent
    {
        public FutureEventScheduledEvent(Guid id,
                                         Guid sourceId,
                                         DateTime raiseTime,
                                         DomainEvent @event,
                                         string sourceName = null,
                                         DateTime? createdTime = null,
                                         Guid? processId = null) : base(sourceId, processId, id, createdTime)
        {
            RaiseTime = raiseTime;
            Event = @event;
            SourceName = sourceName;
        }
        public string SourceName { get; }
        public DateTime RaiseTime { get; }
        public DomainEvent Event { get; }
    }
}