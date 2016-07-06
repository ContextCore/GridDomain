using System;
using GridDomain.CQRS;
using GridDomain.EventSourcing;

namespace GridDomain.Node.FutureEvents
{
    public class RaiseScheduledDomainEventCommand : Command
    {
        public Guid EventId { get; }

        public Guid AggregateId { get; }

        public RaiseScheduledDomainEventCommand(Guid eventId, Guid aggregateId) : base(eventId)
        {
            EventId = eventId;
            AggregateId = aggregateId;
        }
        public RaiseScheduledDomainEventCommand(FutureDomainEvent e) : this(e.SourceId, e.Event.SourceId)
        {
        }
    }
}