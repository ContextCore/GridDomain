using System;
using GridDomain.EventSourcing;

namespace GridDomain.Scheduling.Akka.Messages
{
    public class ScheduledCommandSuccessfullyProcessed : DomainEvent
    {
        public ScheduledCommandSuccessfullyProcessed(Guid sourceId, DateTime? createdTime = null, Guid processId = new Guid())
            : base(sourceId, processId: processId, createdTime: createdTime) {}
    }
}