using System;
using System.Threading.Tasks;
using GridDomain.CQRS;
using GridDomain.Node.AkkaMessaging.Waiting;
using GridDomain.Scheduling;
using GridDomain.Tests.Unit.FutureEvents.Infrastructure;
using Xunit;
using Xunit.Abstractions;

namespace GridDomain.Tests.Unit.FutureEvents
{
    public class Given_aggregate_When_raising_future_event_by_commands : FutureEventsTest
    {
        public Given_aggregate_When_raising_future_event_by_commands(ITestOutputHelper output) : base(output) { }

        [Fact]
        public async Task When_raising_future_event()
        {
            var scheduledTime = DateTime.Now.AddSeconds(1);
            var testCommand = new ScheduleEventInFutureCommand(scheduledTime, Guid.NewGuid(), "test value");

            var waitResults = await Node.Prepare(testCommand)
                                        .Expect<FutureEventScheduledEvent>()
                                        .And<ValueChangedSuccessfullyEvent>()
                                        .Execute();

            var futureEventEnvelop = waitResults.Message<FutureEventScheduledEvent>();
            var producedEvent = waitResults.Message<ValueChangedSuccessfullyEvent>();

            var aggregate = await this.LoadAggregateByActor<TestFutureEventsAggregate>(testCommand.AggregateId);

            //Future_event_fires_in_time()
            Assert.True(scheduledTime.Second - aggregate.ProcessedTime.Second <= 1);
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
        }
    }
}