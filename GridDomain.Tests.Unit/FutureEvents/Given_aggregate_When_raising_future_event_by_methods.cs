using System;
using System.Threading.Tasks;
using GridDomain.Scheduling;
using GridDomain.Tests.Common;
using GridDomain.Tests.Unit.FutureEvents.Infrastructure;
using Xunit;
using Xunit.Abstractions;

namespace GridDomain.Tests.Unit.FutureEvents
{
    public class Given_aggregate_When_raising_future_event_by_methods : FutureEventsTest
    {
        public Given_aggregate_When_raising_future_event_by_methods(ITestOutputHelper output) : base(output) {}

        [Fact]
        public void When_raising_future_event()
        {
            var testCommand = new ScheduleEventInFutureCommand(DateTime.Now, Guid.NewGuid(), "test value");

            var aggregate = new TestFutureEventsAggregate(testCommand.AggregateId);
            aggregate.ScheduleInFuture(testCommand.RaiseTime, testCommand.Value);

            var futureEventEnvelop = aggregate.GetEvent<FutureEventScheduledEvent>();
            aggregate.Commit(futureEventEnvelop);

            //quite ugly, but it only safe way to run some logic after scheduled event persistence
            aggregate.RaiseScheduledEvent(futureEventEnvelop.Id, Guid.NewGuid());
        }

        private static Task AfterScheduledEventOccures(TestFutureEventsAggregate aggregate,
                                                       FutureEventScheduledEvent futureEventEnvelop,
                                                       ScheduleEventInFutureCommand testCommand)
        {
            var producedEvent = aggregate.GetEvent<ValueChangedSuccessfullyEvent>();

            aggregate.Commit(producedEvent);

            var futureEventOccuredEvent = aggregate.GetEvent<FutureEventOccuredEvent>();
            aggregate.Commit(futureEventOccuredEvent);

            //Future_event_occurance_has_same_id_as_future_event()
            Assert.Equal(futureEventEnvelop.Id, futureEventOccuredEvent.FutureEventId);
            //Future_event_applies_to_aggregate()
            Assert.Equal(producedEvent.Value, aggregate.Value);
            //Future_event_envelop_has_id_different_from_aggregate()
            Assert.NotEqual(futureEventEnvelop.Id, aggregate.Id);
            //Future_event_sourceId_is_aggregate_id()
            Assert.Equal(futureEventEnvelop.SourceId, aggregate.Id);
            //Future_event_payload_is_aggregate_original_event()
            Assert.Equal(((ValueChangedSuccessfullyEvent) futureEventEnvelop.Event).Id, producedEvent.Id);
            //Future_event_contains_data_from_command()
            Assert.Equal(testCommand.Value, producedEvent.Value);
            return Task.CompletedTask;
        }
    }
}