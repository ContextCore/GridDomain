using System;
using GridDomain.EventSourcing;

namespace GridDomain.Scheduling.Akka.Messages
{
    public class ScheduledCommandSuccessfullyProcessed : DomainEvent
    {
        public ScheduledCommandSuccessfullyProcessed(Guid sourceId, DateTime? createdTime = null, Guid sagaId = new Guid())
            : base(sourceId, sagaId: sagaId, createdTime: createdTime) {}
    }
}