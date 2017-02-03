using System;
using System.Threading.Tasks;
using GridDomain.CQRS;
using GridDomain.EventSourcing.FutureEvents;
using GridDomain.Node.AkkaMessaging.Waiting;
using GridDomain.Tests.XUnit.FutureEvents.Infrastructure;

namespace GridDomain.Tests.XUnit.FutureEvents
{
    

    public class Given_aggregate_When_raising_future_event_by_commands : FutureEventsTest_InMemory
    {
        private TestAggregate _aggregate;
        private DateTime _scheduledTime;
        private TestDomainEvent _producedEvent;
        private ScheduleEventInFutureCommand _testCommand;
        private FutureEventScheduledEvent _futureEventEnvelop;

        protected override TimeSpan DefaultTimeout => TimeSpan.FromSeconds(10);

      [Fact]

        public async Task When_raising_future_event()
        {
            _scheduledTime = DateTime.Now.AddSeconds(1);
            _testCommand = new ScheduleEventInFutureCommand(_scheduledTime, Guid.NewGuid(), "test value");

            var waitResults = await GridNode.Prepare(_testCommand)
                                            .Expect<FutureEventScheduledEvent>()
                                            .And<TestDomainEvent>()
                                            .Execute();

            _futureEventEnvelop = waitResults.Message<FutureEventScheduledEvent>();
            _producedEvent = waitResults.Message<TestDomainEvent>();

            _aggregate = LoadAggregate<TestAggregate>(_testCommand.AggregateId);
        }

       [Fact]
        public void Future_event_fires_in_time()
        {
            Assert.LessOrEqual(_scheduledTime.Second - _aggregate.ProcessedTime.Second, 1);
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