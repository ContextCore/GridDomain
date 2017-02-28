using System;
using System.Threading.Tasks;
using GridDomain.CQRS;
using GridDomain.EventSourcing.FutureEvents;
using GridDomain.Node.AkkaMessaging.Waiting;
using GridDomain.Tests.XUnit.FutureEvents.Infrastructure;
using Xunit;
using Xunit.Abstractions;

namespace GridDomain.Tests.XUnit.FutureEvents
{
    public class Given_different_aggregates_future_events_Should_be_processed_independently : FutureEventsTest
    {
        public Given_different_aggregates_future_events_Should_be_processed_independently(ITestOutputHelper output)
            : base(output) {}

        [Fact]
        public async Task Raising_several_future_events_for_different_aggregates()
        {
            var commandA = new ScheduleEventInFutureCommand(DateTime.Now.AddSeconds(0.5), Guid.NewGuid(), "test value A");
            var commandB = new ScheduleEventInFutureCommand(DateTime.Now.AddSeconds(0.5), Guid.NewGuid(), "test value B");

            var eventA =
                (await Node.Prepare(commandA).Expect<FutureEventOccuredEvent>().Execute()).Message<FutureEventOccuredEvent>();

            var eventB =
                (await Node.Prepare(commandB).Expect<FutureEventOccuredEvent>().Execute()).Message<FutureEventOccuredEvent>();

            //Future_event_ids_are_different()
            Assert.NotEqual(eventA.FutureEventId, eventB.FutureEventId);
            //EventA_is_result_of_commandA()
            Assert.Equal(eventA.SourceId, commandA.AggregateId);
            //EventB_is_result_of_commandB()
            Assert.Equal(eventB.SourceId, commandB.AggregateId);
        }
    }
}