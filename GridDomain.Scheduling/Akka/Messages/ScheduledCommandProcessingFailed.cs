using System;
using GridDomain.EventSourcing;

namespace GridDomain.Scheduling.Akka.Messages
{
    public class ScheduledCommandProcessingFailed : DomainEvent
    {
        public Exception Exception { get; private set; }

        public ScheduledCommandProcessingFailed(Guid sourceId, Exception exception,  DateTime? createdTime = null, Guid sagaId = new Guid()) : base(sourceId, createdTime, sagaId)
        {
            Exception = exception;
        }
    }
}