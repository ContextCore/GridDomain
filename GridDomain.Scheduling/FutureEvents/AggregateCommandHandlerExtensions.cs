using GridDomain.EventSourcing;

namespace GridDomain.Scheduling.FutureEvents
{
    public class FutureEventsAggregateCommandHandler<TAggregate> : AggregateCommandsHandler<TAggregate> where TAggregate : FutureEventsAggregate
    {
        public FutureEventsAggregateCommandHandler()
        {
            Map<RaiseScheduledDomainEventCommand>((c, a) => a.RaiseScheduledEvent(c.FutureEventId, c.Id));
        }
    }
}