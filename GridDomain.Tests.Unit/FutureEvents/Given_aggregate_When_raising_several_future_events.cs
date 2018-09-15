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
    public class Given_aggregate_When_raising_several_future_events : NodeTestKit
    {
        public Given_aggregate_When_raising_several_future_events(ITestOutputHelper output) : this(new FutureEventsFixture(output)) { }
        protected Given_aggregate_When_raising_several_future_events(NodeTestFixture fixture) : base(fixture) { }

        [Fact]
        public async Task FutureDomainEvent_envelops_has_unique_id()
        {
            var aggregateId = Guid.NewGuid()
                                  .ToString();

            var eventA =
                await Node.Prepare(new ScheduleEventInFutureCommand(DateTime.Now.AddSeconds(0.1), aggregateId, "test value A"))
                          .Expect<FutureEventOccuredEvent>()
                          .Execute()
                          .Message();

            var eventB =
                await Node.Prepare(new ScheduleEventInFutureCommand(DateTime.Now.AddSeconds(0.1), aggregateId, "test value B"))
                          .Expect<FutureEventOccuredEvent>()
                          .Execute()
                          .Message();

            //Envelop_ids_are_different()
            Assert.NotEqual(eventA.FutureEventId, eventB.FutureEventId);
            //Envelop_id_not_equal_to_aggregate_id()
            Assert.True(eventA.Id != aggregateId && aggregateId != eventB.Id);
        }
    }
}