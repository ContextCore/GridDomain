using System;
using System.Linq;
using GridDomain.EventSourcing.Sagas.FutureEvents;
using GridDomain.Tests.FutureEvents.Infrastructure;
using NUnit.Framework;

namespace GridDomain.Tests.FutureEvents
{
    [TestFixture]

    public class Given_aggregate_When_raising_future_event_by_commands : FutureEventsTest_InMemory
    {
        private TestAggregate _aggregate;
        private DateTime _scheduledTime;
        private TestDomainEvent _producedEvent;
        private RaiseEventInFutureCommand _testCommand;
        private FutureEventScheduledEvent _futureEventEnvelop;

        protected override TimeSpan Timeout => TimeSpan.FromSeconds(5);

        [OneTimeSetUp]

        public void When_raising_future_event()
        {
            _scheduledTime = DateTime.Now.AddSeconds(1);
            _testCommand = new RaiseEventInFutureCommand(_scheduledTime, Guid.NewGuid(), "test value");

            _futureEventEnvelop = (FutureEventScheduledEvent)ExecuteAndWaitFor<FutureEventScheduledEvent>(_testCommand).Received.First();
            _producedEvent =  (TestDomainEvent)WaitFor<TestDomainEvent>().Received.First();
            _aggregate = LoadAggregate<TestAggregate>(_testCommand.AggregateId);
        }

        [Then]
        public void Future_event_fires_in_time()
        {
            Assert.LessOrEqual(_scheduledTime.Second - _aggregate.ProcessedTime.Second, 1);
        }

        [Then]
        public void Future_event_applies_to_aggregate()
        {
            Assert.AreEqual(_producedEvent.Value, _aggregate.Value);
        }

        [Then]
        public void Future_event_envelop_has_id_different_from_aggregate()
        {
            Assert.AreNotEqual(_futureEventEnvelop.Id, _aggregate.Value);
        }

        [Then]
        public void Future_event_sourceId_is_aggregate_id()
        {
            Assert.AreEqual(_futureEventEnvelop.SourceId, _aggregate.Id);
        }

        [Then]
        public void Future_event_payload_is_aggregate_original_event()
        {
            Assert.AreEqual(((TestDomainEvent)_futureEventEnvelop.Event).Id, _producedEvent.Id);
        }

        [Then]
        public void Future_event_contains_data_from_command()
        {
            Assert.AreEqual(_testCommand.Value, _producedEvent.Value);
        }
    }
}