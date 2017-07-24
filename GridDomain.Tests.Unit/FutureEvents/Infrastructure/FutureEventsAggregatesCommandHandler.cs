using GridDomain.Scheduling;

namespace GridDomain.Tests.Unit.FutureEvents.Infrastructure
{
    public class FutureEventsAggregatesCommandHandler : FutureEventsAggregateCommandHandler<TestFutureEventsAggregate>
    {
        public FutureEventsAggregatesCommandHandler()
        {
            Map<ScheduleEventInFutureCommand>((c, a) => a.ScheduleInFuture(c.RaiseTime, c.Value));
            Map<CancelFutureEventCommand>((c, a) => a.CancelFutureEvents(c.Value));
            Map<BoomNowCommand>((c, a) => a.Boom());
            Map<PlanBoomCommand>((c, a) => a.PlanBoom(c.BoomTime));
        }
    }
}