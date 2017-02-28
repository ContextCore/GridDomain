using System;
using GridDomain.EventSourcing;

namespace GridDomain.Scheduling.Akka.Messages
{
    public class ScheduledCommandProcessingFailed : DomainEvent
    {
        public ScheduledCommandProcessingFailed(Guid sourceId, Exception exception) : base(sourceId)
        {
            Exception = exception;
        }

        public Exception Exception { get; }
    }
}