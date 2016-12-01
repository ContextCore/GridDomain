using System;
using GridDomain.EventSourcing;

namespace GridDomain.Scheduling.Akka.Messages
{
    public class ScheduledCommandSuccessfullyProcessed : DomainEvent
    {
        public ScheduledCommandSuccessfullyProcessed(Guid sourceId, DateTime? createdTime = null, Guid sagaId = new Guid()) : base(sourceId, createdTime, sagaId)
        {
        }
    }

    public class ScheduledCommandProcessingFailuresPlanned : DomainEvent
    {
        public int FailTimes { get; }

        public ScheduledCommandProcessingFailuresPlanned(Guid sourceId, int failTimes) : base(sourceId)
        {
            FailTimes = failTimes;
        }
    }
}