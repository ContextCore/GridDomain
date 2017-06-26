using System;
using System.Threading.Tasks;
using GridDomain.CQRS;
using GridDomain.EventSourcing.FutureEvents;
using GridDomain.Node.AkkaMessaging.Waiting;
using GridDomain.Tests.XUnit.FutureEvents;
using GridDomain.Tests.XUnit.FutureEvents.Infrastructure;
using Xunit;
using Xunit.Abstractions;

namespace GridDomain.Tests.Acceptance.XUnit.FutureDomainEvents
{
    public class Given_aggregate_When_raising_several_future_events_Persistent : FutureEventsTest
    {
        public Given_aggregate_When_raising_several_future_events_Persistent(ITestOutputHelper output) : base(output) {}

        [Fact]
        public async Task FutureDomainEvent_envelops_has_unique_id()
        {
            var aggregateId = Guid.NewGuid();
            var testCommandA = new ScheduleEventInFutureCommand(DateTime.Now.AddSeconds(2), aggregateId, "test value A");
            var testCommandB = new ScheduleEventInFutureCommand(DateTime.Now.AddSeconds(5), aggregateId, "test value B");

            var eventA =
                (await Node.Prepare(testCommandA).Expect<FutureEventOccuredEvent>().Execute())
                .Message<FutureEventOccuredEvent>();

            var eventB =
                (await Node.Prepare(testCommandB).Expect<FutureEventOccuredEvent>().Execute())
                .Message<FutureEventOccuredEvent>();

            //Envelop_ids_are_different()
            Assert.NotEqual(eventA.FutureEventId, eventB.FutureEventId);
            //Envelop_id_not_equal_to_aggregate_id()
            Assert.True(eventA.Id != aggregateId && aggregateId != eventB.Id);
        }
    }
}