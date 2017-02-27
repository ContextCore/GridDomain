using System;
using GridDomain.CQRS;

namespace GridDomain.EventSourcing.FutureEvents
{
    public class RaiseScheduledDomainEventCommand : Command
    {
        public Guid FutureEventId { get; }

        public RaiseScheduledDomainEventCommand(Guid futureEventId, Guid aggregateId, Guid id):base(id,aggregateId)
        {
            FutureEventId = futureEventId;
        }
    }
}