using GridDomain.EventSourcing;

namespace GridDomain.Scheduling.FutureEvents
{
    public static class AggregateCommandHandlerExtensions
    {
        public static void MapFutureEvents<TAggregate>(this AggregateCommandsHandler<TAggregate> handler)
            where TAggregate : FutureEventsAggregate
        {
            handler.Map<RaiseScheduledDomainEventCommand>((c, a) => a.RaiseScheduledEvent(c.FutureEventId, c.Id));
        }
    }
}