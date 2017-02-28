using System;
using System.Linq;
using GridDomain.EventSourcing;
using GridDomain.EventSourcing.FutureEvents;
using GridDomain.Tests.Framework;
using GridDomain.Tests.XUnit.FutureEvents.Infrastructure;
using Xunit;

namespace GridDomain.Tests.XUnit.FutureEvents.Cancelation
{
    public class Given_aggregate_When_cancel_existing_future_event
    {
        [Fact]
        public void When_cancel_existing_scheduled_future_event()
        {
            var aggregate = new FutureEventsAggregate(Guid.NewGuid());
            var testValue = "value D";

            aggregate.ScheduleInFuture(DateTime.Now.AddSeconds(400), testValue);
            aggregate.ScheduleInFuture(DateTime.Now.AddSeconds(400), "test value E");

            var futureEventA = aggregate.GetEvents<FutureEventScheduledEvent>().First();

            var futureEventOutOfCriteria = aggregate.GetEvents<FutureEventScheduledEvent>().Skip(1).First();

            aggregate.ClearEvents();
            aggregate.CancelFutureEvents(testValue);
            // Cancelation_event_is_produced()
            var cancelEvent = aggregate.GetEvent<FutureEventCanceledEvent>();
            Assert.Equal(futureEventA.Id, cancelEvent.FutureEventId);
            //Only_predicate_satisfying_events_are_canceled()
            var cancelEvents = aggregate.GetEvents<FutureEventCanceledEvent>();
            Assert.True(cancelEvents.All(e => e.FutureEventId != futureEventOutOfCriteria.Id));
            // Canceled_event_cannot_be_raised()
            aggregate.ClearEvents();
            Assert.Throws<ScheduledEventNotFoundException>(
                () => aggregate.RaiseScheduledEvent(futureEventA.Id, Guid.NewGuid()));
            var anyEvents = aggregate.GetEvents<DomainEvent>();
            Assert.Empty(anyEvents);
        }
    }
}