using System;
using GridDomain.CQRS;

namespace GridDomain.Node.FutureEvents
{
    public class RaiseScheduledDomainEventCommand : Command
    {
        public Guid EventId;

        public Guid AggregateId;

        public RaiseScheduledDomainEventCommand(Guid id, Guid eventId, Guid aggregateId) : base(id)
        {
            EventId = eventId;
            AggregateId = aggregateId;
        }
    }
}