using System;
using System.Threading.Tasks;
using GridDomain.CQRS;
using GridDomain.Scheduling;
using GridDomain.Tests.Unit.FutureEvents.Infrastructure;
using Xunit;
using Xunit.Abstractions;

namespace GridDomain.Tests.Unit.FutureEvents
{
    public class Given_different_aggregates_future_events_Should_be_processed_independently : NodeTestKit
    {
        public Given_different_aggregates_future_events_Should_be_processed_independently(ITestOutputHelper output) : this(new FutureEventsFixture(output)) { }
        protected Given_different_aggregates_future_events_Should_be_processed_independently(NodeTestFixture fixture) : base(fixture) { }

        [Fact]
        public async Task Raising_several_future_events_for_different_aggregates()
        {
            var commandA = new ScheduleEventInFutureCommand(DateTime.Now.AddSeconds(0.5), Guid.NewGuid().ToString(), "test value A");
            var commandB = new ScheduleEventInFutureCommand(DateTime.Now.AddSeconds(0.5), Guid.NewGuid().ToString(), "test value B");

            var eventA =
                await Node.Prepare(commandA).Expect<FutureEventOccuredEvent>().Execute().Received();

            var eventB =
                await Node.Prepare(commandB).Expect<FutureEventOccuredEvent>().Execute().Received();

            //Future_event_ids_are_different()
            Assert.NotEqual(eventA.FutureEventId, eventB.FutureEventId);
            //EventA_is_result_of_commandA()
            Assert.Equal(eventA.SourceId, commandA.AggregateId);
            //EventB_is_result_of_commandB()
            Assert.Equal(eventB.SourceId, commandB.AggregateId);
        }
    }
}