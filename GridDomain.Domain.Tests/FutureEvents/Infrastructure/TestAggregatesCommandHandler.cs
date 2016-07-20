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
            Map<TestCommand>(c => c.AggregateId,
                            (c, a) => a.ScheduleInFuture(c.RaiseTime));

            this.MapFutureEvents();
        }

        public Type AggregateType => typeof(TestAggregate);
    }
}