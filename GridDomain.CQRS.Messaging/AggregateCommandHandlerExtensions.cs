using GridDomain.CQRS.Messaging.MessageRouting;
using GridDomain.EventSourcing.Sagas.FutureEvents;

namespace GridDomain.Node.FutureEvents
{
    public static class AggregateCommandHandlerExtensions
    {
        public static void MapFutureEvents<TAggregate>(this AggregateCommandsHandler<TAggregate> handler) where TAggregate : Aggregate
        {
            handler.Map<RaiseScheduledDomainEventCommand>(c => c.AggregateId,
                                                         (c, a) => a.RaiseScheduledEvent(c.AggregateId));
        }
    }
}