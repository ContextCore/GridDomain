using System;
using GridDomain.CQRS.Messaging;
using GridDomain.CQRS.Messaging.MessageRouting;

namespace GridDomain.Tests.FutureEvents.Infrastructure
{
    public class TestAggregatesCommandHandler : AggregateCommandsHandler<TestAggregate>,
                                                IAggregateCommandsHandlerDesriptor

    {
        public static readonly IAggregateCommandsHandlerDesriptor Descriptor = new TestAggregatesCommandHandler();
        public TestAggregatesCommandHandler() : base(null)
        {
            Map<ScheduleEventInFutureCommand>(c => c.AggregateId,
                                             (c, a) => a.ScheduleInFuture(c.RaiseTime, c.Value));

            Map<ScheduleErrorInFutureCommand>(c => c.AggregateId,
                                            (c, a) => a.ScheduleErrorInFuture(c.RaiseTime, c.Value, c.SuccedOnRetryNum));

            Map<CancelFutureEventCommand>(c => c.AggregateId,
                                          (c, a) => a.CancelFutureEvents(c.Value));

            this.MapFutureEvents();
        }

        public Type AggregateType => typeof(TestAggregate);
    }
}