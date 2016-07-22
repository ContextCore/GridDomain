using System;
using GridDomain.EventSourcing;
using GridDomain.EventSourcing.Sagas.FutureEvents;
using GridDomain.Tests.FutureEvents.Infrastructure;
using NUnit.Framework;

namespace GridDomain.Tests.FutureEvents.Cancelation
{
    [TestFixture]
    public class Given_aggregate_When_cancel_not_existing_future_event
    {
        private TestAggregate _aggregate;
        private FutureEventScheduledEvent _futureEvent;

        [SetUp]
        public void When_cancel_existing_scheduled_future_event()
        {
            _aggregate = new TestAggregate(Guid.NewGuid());
            var testValue = "value D";

            _aggregate.ScheduleInFuture(DateTime.Now.AddSeconds(400), testValue);
            _futureEvent = _aggregate.GetEvent<FutureEventScheduledEvent>();
            _aggregate.ClearEvents();
            _aggregate.CancelFutureEvents("will not be found in any future event");
        }

        [Then]
        public void No_events_are_produced()
        {
            CollectionAssert.IsEmpty(_aggregate.GetEvents<DomainEvent>());
        }

        [Then]
        public void All_existed_future_events_remain_the_same()
        {
            _aggregate.RaiseScheduledEvent(_futureEvent.Id);
            var occuredEvent = _aggregate.GetEvent<FutureEventOccuredEvent>();
            Assert.AreEqual(_futureEvent.Id, occuredEvent.FutureEventId);
        }
    }
}