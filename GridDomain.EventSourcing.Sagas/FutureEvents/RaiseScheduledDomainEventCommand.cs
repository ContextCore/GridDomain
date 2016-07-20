using System;
using GridDomain.CQRS;

namespace GridDomain.EventSourcing.Sagas.FutureEvents
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