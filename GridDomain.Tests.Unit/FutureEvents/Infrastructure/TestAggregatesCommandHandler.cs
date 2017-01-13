using System;
using GridDomain.CQRS.Messaging;
using GridDomain.CQRS.Messaging.MessageRouting;

namespace GridDomain.Tests.Unit.FutureEvents.Infrastructure
{
    public class TestAggregatesCommandHandler : AggregateCommandsHandler<TestAggregate>,
                                                IAggregateCommandsHandlerDescriptor

    {
        public static readonly IAggregateCommandsHandlerDescriptor Descriptor = new TestAggregatesCommandHandler();
        public TestAggregatesCommandHandler() : base()
        {
            Map<ScheduleEventInFutureCommand>(c => c.AggregateId,
                                             (c, a) => a.ScheduleInFuture(c.RaiseTime, c.Value));

            Map<ScheduleErrorInFutureCommand>(c => c.AggregateId,
                                            (c, a) => a.ScheduleErrorInFuture(c.RaiseTime, c.Value, c.SuccedOnRetryNum));

            Map<CancelFutureEventCommand>(c => c.AggregateId,
                                          (c, a) => a.CancelFutureEvents(c.Value));

            Map<ScheduleErrorInFutureCommand>(c => c.AggregateId,
                                              (c, a) => a.ScheduleErrorInFuture(c.RaiseTime,c.Value, c.SuccedOnRetryNum));

            this.MapFutureEvents();
        }

        public Type AggregateType => typeof(TestAggregate);
    }
}