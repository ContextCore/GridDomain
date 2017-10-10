using System;
using GridDomain.EventSourcing;
using GridDomain.Tests.Common;
using GridDomain.Tests.Unit.FutureEvents.Infrastructure;
using GridDomain.Tools;
using Xunit;

namespace GridDomain.Tests.Unit.FutureEvents.Cancelation
{
    public class Given_aggregate_When_cancel_not_existing_future_event
    {
        [Fact]
        public void Then_nothing_happened()
        {
            var aggregate = new TestFutureEventsAggregate(Guid.NewGuid());
            var testValue = "value D";

            aggregate.ScheduleInFuture(DateTime.Now.AddSeconds(400), testValue);

            aggregate.CommitAll();

            aggregate.CancelFutureEvents("will not be found in any future event");
           //No_events_were_produced()
            Assert.Empty(aggregate.GetEvents<DomainEvent>());
        }
    }
}