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
        public void When_cancel_existing_scheduled_future_event()
        {
            var aggregate = new FutureEventsAggregate(Guid.NewGuid());
            var testValue = "value D";

            aggregate.ScheduleInFuture(DateTime.Now.AddSeconds(400), testValue);
            var futureEvent = aggregate.GetEvent<FutureEventScheduledEvent>();
            aggregate.ClearEvents();
            aggregate.CancelFutureEvents("will not be found in any future event");

            //No_events_are_produced()
            Assert.Empty(aggregate.GetEvents<DomainEvent>());
            //All_existed_future_events_remain_the_same()
            aggregate.RaiseScheduledEvent(futureEvent.Id, Guid.NewGuid());
            var occuredEvent = aggregate.GetEvent<FutureEventOccuredEvent>();
            Assert.Equal(futureEvent.Id, occuredEvent.FutureEventId);
        }
    }
}