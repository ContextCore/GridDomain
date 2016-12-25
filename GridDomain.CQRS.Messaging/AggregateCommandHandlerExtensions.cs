using GridDomain.CQRS.Messaging.MessageRouting;
using GridDomain.EventSourcing;
using GridDomain.EventSourcing.FutureEvents;

namespace GridDomain.CQRS.Messaging
{
    public static class AggregateCommandHandlerExtensions
    {
        public static void MapFutureEvents<TAggregate>(this AggregateCommandsHandler<TAggregate> handler) where TAggregate : Aggregate
        {
            handler.Map<RaiseScheduledDomainEventCommand>(c => c.AggregateId,
                                                         (c, a) => a.RaiseScheduledEvent(c.FutureEventId));
        }
    }
}