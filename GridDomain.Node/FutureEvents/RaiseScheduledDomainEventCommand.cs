using System;
using GridDomain.CQRS;

namespace GridDomain.Node.FutureEvents
{
    public class RaiseScheduledDomainEventCommand : Command
    {
        public Guid EventId { get; }

        public Guid AggregateId { get; }

        public RaiseScheduledDomainEventCommand(Guid id, Guid eventId, Guid aggregateId) : base(id)
        {
            EventId = eventId;
            AggregateId = aggregateId;
        }
    }
}