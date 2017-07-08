using System;
using GridDomain.EventSourcing;
using GridDomain.Tests.Common;
using GridDomain.Tests.Unit.DependencyInjection.FutureEvents.Infrastructure;
using Xunit;

namespace GridDomain.Tests.Unit.DependencyInjection.FutureEvents.Cancelation
{
    public class Given_aggregate_When_cancel_not_existing_future_event
    {
        [Fact]
        public void Then_nothing_happened()
        {
            var aggregate = new TestFutureEventsAggregate(Guid.NewGuid());
            var testValue = "value D";

            aggregate.ScheduleInFuture(DateTime.Now.AddSeconds(400), testValue);

            aggregate.ClearEvents();

            aggregate.CancelFutureEvents("will not be found in any future event");
           //No_events_were_produced()
            Assert.Empty(aggregate.GetEvents<DomainEvent>());
        }
    }
}