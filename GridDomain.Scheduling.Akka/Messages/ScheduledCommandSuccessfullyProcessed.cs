using System;
using GridDomain.EventSourcing;

namespace GridDomain.Scheduling.Akka.Messages
{
    public class ScheduledCommandSuccessfullyProcessed : DomainEvent
    {
        public ScheduledCommandSuccessfullyProcessed(string sourceId, DateTime? createdTime = null, string processId = null)
            : base(sourceId, processId: processId, createdTime: createdTime) {}
    }
}