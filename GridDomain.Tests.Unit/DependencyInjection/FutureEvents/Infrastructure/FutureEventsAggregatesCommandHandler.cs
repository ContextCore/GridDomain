using System;
using GridDomain.CQRS.Messaging;
using GridDomain.EventSourcing;

namespace GridDomain.Tests.Unit.DependencyInjection.FutureEvents.Infrastructure
{
    public class FutureEventsAggregatesCommandHandler : AggregateCommandsHandler<FutureEventsAggregate>,
                                                        IAggregateCommandsHandlerDescriptor

    {
        public FutureEventsAggregatesCommandHandler()
        {
            Map<ScheduleEventInFutureCommand>((c, a) => a.ScheduleInFuture(c.RaiseTime, c.Value));

            Map<ScheduleErrorInFutureCommand>((c, a) => a.ScheduleErrorInFuture(c.RaiseTime, c.Value, c.SuccedOnRetryNum));

            Map<CancelFutureEventCommand>((c, a) => a.CancelFutureEvents(c.Value));

            this.MapFutureEvents();
        }
    }
}