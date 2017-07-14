using System;
using GridDomain.CQRS.Messaging;
using GridDomain.EventSourcing;
using GridDomain.Scheduling.FutureEvents;
using GridDomain.Tests.Unit.FutureEvents.Infrastructure;

namespace GridDomain.Tests.Unit.DependencyInjection.FutureEvents.Infrastructure
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