using System;
using GridDomain.CQRS;

namespace GridDomain.EventSourcing.FutureEvents
{
    public class RaiseScheduledDomainEventCommand : Command
    {
        public RaiseScheduledDomainEventCommand(Guid futureEventId, Guid aggregateId, Guid id) : base(id, aggregateId)
        {
            FutureEventId = futureEventId;
        }

        public Guid FutureEventId { get; }
    }
}