using System;
using GridDomain.EventSourcing.FutureEvents;
using GridDomain.Tests.Framework;
using GridDomain.Tests.XUnit.FutureEvents.Infrastructure;
using Xunit;
using Xunit.Abstractions;

namespace GridDomain.Tests.XUnit.FutureEvents
{
    public class Given_aggregate_When_raising_future_event_by_methods : FutureEventsTest
    {
        public Given_aggregate_When_raising_future_event_by_methods(ITestOutputHelper output) : base(output) {}

        [Fact]
        public void When_raising_future_event()
        {
            var testCommand = new ScheduleEventInFutureCommand(DateTime.Now, Guid.NewGuid(), "test value");

            var aggregate = new FutureEventsAggregate(testCommand.AggregateId);
            aggregate.ScheduleInFuture(testCommand.RaiseTime, testCommand.Value);

            var futureEventEnvelop = aggregate.GetEvent<FutureEventScheduledEvent>();
            aggregate.RaiseScheduledEvent(futureEventEnvelop.Id, Guid.NewGuid());

            var producedEvent = aggregate.GetEvent<TestDomainEvent>();
            var futureEventOccuredEvent = aggregate.GetEvent<FutureEventOccuredEvent>();

            //Future_event_occurance_has_same_id_as_future_event()
            Assert.Equal(futureEventEnvelop.Id, futureEventOccuredEvent.FutureEventId);
            //Future_event_applies_to_aggregate()
            Assert.Equal(producedEvent.Value, aggregate.Value);
            //Future_event_envelop_has_id_different_from_aggregate()
            Assert.NotEqual(futureEventEnvelop.Id, aggregate.Id);
            //Future_event_sourceId_is_aggregate_id()
            Assert.Equal(futureEventEnvelop.SourceId, aggregate.Id);
            //Future_event_payload_is_aggregate_original_event()
            Assert.Equal(((TestDomainEvent) futureEventEnvelop.Event).Id, producedEvent.Id);
            //Future_event_contains_data_from_command()
            Assert.Equal(testCommand.Value, producedEvent.Value);
        }
    }
}