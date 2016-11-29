using System;
using GridDomain.EventSourcing;

namespace GridDomain.Scheduling.Akka.Messages
{
    public class ScheduledCommandProcessingFailed : DomainEvent
    {
        public Exception Exception { get;}

        public ScheduledCommandProcessingFailed(Guid sourceId, Exception exception) : base(sourceId)
        {
            Exception = exception;
        }
    }
}