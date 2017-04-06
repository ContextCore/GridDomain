using System;
using GridDomain.CQRS.Messaging;
using GridDomain.CQRS.Messaging.MessageRouting;
using GridDomain.EventSourcing;

namespace GridDomain.Tests.XUnit.FutureEvents.Infrastructure
{
    public class FutureEventsAggregatesCommandHandler : AggregateCommandsHandler<FutureEventsAggregate>,
                                                        IAggregateCommandsHandlerDescriptor

    {
        public static readonly IAggregateCommandsHandlerDescriptor Descriptor = new FutureEventsAggregatesCommandHandler();

        public FutureEventsAggregatesCommandHandler()
        {
            Map<ScheduleEventInFutureCommand>((c, a) => a.ScheduleInFuture(c.RaiseTime, c.Value));

            Map<ScheduleErrorInFutureCommand>((c, a) => a.ScheduleErrorInFuture(c.RaiseTime, c.Value, c.SuccedOnRetryNum));

            Map<CancelFutureEventCommand>((c, a) => a.CancelFutureEvents(c.Value));

            this.MapFutureEvents();
        }

        public Type AggregateType => typeof(FutureEventsAggregate);
    }
}