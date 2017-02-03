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
        private TestAggregate _aggregate;
        private FutureEventScheduledEvent _futureEventA;
        private FutureEventScheduledEvent _futureEvent_out_of_criteria;

        [Fact]
        public void When_cancel_existing_scheduled_future_event()
        {
            _aggregate = new TestAggregate(Guid.NewGuid());
            var testValue = "value D";

            _aggregate.ScheduleInFuture(DateTime.Now.AddSeconds(400), testValue);
            _aggregate.ScheduleInFuture(DateTime.Now.AddSeconds(400), "test value E");

            _futureEventA = _aggregate.GetEvents<FutureEventScheduledEvent>().First();
            _futureEvent_out_of_criteria = _aggregate.GetEvents<FutureEventScheduledEvent>().Skip(1).First();

            _aggregate.ClearEvents();
            _aggregate.CancelFutureEvents(testValue);
        }

       [Fact]
        public void Cancelation_event_is_produced()
        {
            var cancelEvent = _aggregate.GetEvent<FutureEventCanceledEvent>();
           Assert.Equal(_futureEventA.Id, cancelEvent.FutureEventId);
        }

       [Fact]
        public void Only_predicate_satisfying_events_are_canceled()
        {
            var cancelEvent = _aggregate.GetEvents<FutureEventCanceledEvent>();
            Assert.True(cancelEvent.All(e => e.FutureEventId != _futureEvent_out_of_criteria.Id));
        }

       [Fact]
        public void Canceled_event_cannot_be_raised()
        {
            _aggregate.ClearEvents();
            Assert.Throws<ScheduledEventNotFoundException>(() => _aggregate.RaiseScheduledEvent(_futureEventA.Id,Guid.NewGuid()));
            var anyEvents = _aggregate.GetEvents<DomainEvent>();
            Assert.Empty(anyEvents);
        }
    }
}