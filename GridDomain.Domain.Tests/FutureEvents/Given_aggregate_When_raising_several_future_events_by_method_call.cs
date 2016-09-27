using System;
using CommonDomain;
using GridDomain.EventSourcing.Sagas.FutureEvents;
using GridDomain.Tests.FutureEvents.Infrastructure;
using NUnit.Framework;

namespace GridDomain.Tests.FutureEvents
{
    [TestFixture]
    public class Given_aggregate_When_raising_several_future_events_by_method_call
    {
        private TestAggregate _aggregate;

        [OneTimeSetUp]
        public void When_scheduling_future_event()
        {
            _aggregate = new TestAggregate(Guid.NewGuid());
            _aggregate.ScheduleInFuture(DateTime.Now.AddSeconds(400),"value D");
            ((IAggregate)_aggregate).ClearUncommittedEvents();
        }

        [Then]
        public void Then_raising_event_with_wrong_id_throws_an_error()
        {
            Assert.Throws<ScheduledEventNotFoundException>(() => _aggregate.RaiseScheduledEvent(Guid.NewGuid()));
        }

        [Then]
        public void Then_raising_event_with_wrong_id_does_not_produce_new_events()
        {
            try
            {
                _aggregate.RaiseScheduledEvent(Guid.NewGuid());
            }
            catch
            {
                //intentionally left empty
            }
            CollectionAssert.IsEmpty(((IAggregate)_aggregate).GetUncommittedEvents());
        }
    }
}