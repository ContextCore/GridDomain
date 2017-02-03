using System;
using GridDomain.EventSourcing.FutureEvents;
using GridDomain.Tests.Framework;
using GridDomain.Tests.XUnit.FutureEvents.Infrastructure;

namespace GridDomain.Tests.XUnit.FutureEvents
{
    

    public class Given_aggregate_When_raising_future_event_by_methods : FutureEventsTest_InMemory
    {
        private TestAggregate _aggregate;
        private TestDomainEvent _producedEvent;
        private ScheduleEventInFutureCommand _testCommand;
        private FutureEventScheduledEvent _futureEventEnvelop;
        private FutureEventOccuredEvent _futureEventOccuredEvent;

      [Fact]

        public void When_raising_future_event()
        {
            _testCommand = new ScheduleEventInFutureCommand(DateTime.Now, Guid.NewGuid(), "test value");

            _aggregate = new TestAggregate(_testCommand.AggregateId);
            _aggregate.ScheduleInFuture(_testCommand.RaiseTime, _testCommand.Value);

            _futureEventEnvelop = _aggregate.GetEvent<FutureEventScheduledEvent>();
            _aggregate.RaiseScheduledEvent(_futureEventEnvelop.Id, Guid.NewGuid());
            _producedEvent = _aggregate.GetEvent<TestDomainEvent>();
            _futureEventOccuredEvent = _aggregate.GetEvent<FutureEventOccuredEvent>();
        }

       [Fact]
        public void Future_event_occurance_has_same_id_as_future_event()
        {
           Assert.Equal(_futureEventEnvelop.Id, _futureEventOccuredEvent.FutureEventId);
        }

       [Fact]
        public void Future_event_applies_to_aggregate()
        {
           Assert.Equal(_producedEvent.Value, _aggregate.Value);
        }

       [Fact]
        public void Future_event_envelop_has_id_different_from_aggregate()
        {
            Assert.AreNotEqual(_futureEventEnvelop.Id, _aggregate.Value);
        }

       [Fact]
        public void Future_event_sourceId_is_aggregate_id()
        {
           Assert.Equal(_futureEventEnvelop.SourceId, _aggregate.Id);
        }

       [Fact]
        public void Future_event_payload_is_aggregate_original_event()
        {
           Assert.Equal(((TestDomainEvent)_futureEventEnvelop.Event).Id, _producedEvent.Id);
        }

       [Fact]
        public void Future_event_contains_data_from_command()
        {
           Assert.Equal(_testCommand.Value, _producedEvent.Value);
        }
    }
}