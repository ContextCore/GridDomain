using System;
using GridDomain.CQRS.Messaging.MessageRouting;
using GridDomain.Node.FutureEvents;

namespace GridDomain.Tests.FutureEvents.Infrastructure
{
    public class TestAggregatesCommandHandler : AggregateCommandsHandler<TestAggregate>,
                                                IAggregateCommandsHandlerDesriptor

    {
        public static readonly IAggregateCommandsHandlerDesriptor Descriptor = new TestAggregatesCommandHandler();
        public TestAggregatesCommandHandler() : base(null)
        {
            Map<RaiseEventInFutureCommand>(c => c.AggregateId,
                                          (c, a) => a.ScheduleInFuture(c.RaiseTime, c.Value));

            Map<CancelFutureEventCommand>(c => c.AggregateId,
                                          (c, a) => a.CancelFutureEvents(c.Value));

            this.MapFutureEvents();
        }

        public Type AggregateType => typeof(TestAggregate);
    }
}