using System;
using GridDomain.EventSourcing;

namespace GridDomain.Scheduling.Akka.Messages
{
    public class ScheduledCommandProcessingFailuresPlanned : DomainEvent
    {
        public int FailTimes { get; }

        public ScheduledCommandProcessingFailuresPlanned(Guid sourceId, int failTimes) : base(sourceId)
        {
            FailTimes = failTimes;
        }
    }
}