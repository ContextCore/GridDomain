using System;
using GridDomain.EventSourcing;
using GridDomain.EventSourcing.FutureEvents;
using GridDomain.Tests.Framework;
using GridDomain.Tests.XUnit.FutureEvents.Infrastructure;
using Xunit;

namespace GridDomain.Tests.XUnit.FutureEvents.Cancelation
{
    public class Given_aggregate_When_cancel_not_existing_future_event
    {
        [Fact]
        public void Then_not_found_exception_occures()
        {
            var aggregate = new FutureEventsAggregate(Guid.NewGuid());
            var testValue = "value D";

            aggregate.ScheduleInFuture(DateTime.Now.AddSeconds(400), testValue);
            var futureEvent = aggregate.GetEvent<FutureEventScheduledEvent>();
            aggregate.ClearEvents();
            aggregate.CancelFutureEvents("will not be found in any future event");

         
            Assert.Throws<ScheduledEventNotFoundException>(() => aggregate.RaiseScheduledEvent(futureEvent.Id, Guid.NewGuid()));

            //No_events_were_produced()
            Assert.Empty(aggregate.GetEvents<DomainEvent>());
        }
    }
}