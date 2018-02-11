using System;
using System.Threading.Tasks;
using GridDomain.CQRS;
using GridDomain.Node.AkkaMessaging.Waiting;
using GridDomain.Scheduling;
using GridDomain.Tests.Unit.FutureEvents;
using GridDomain.Tests.Unit.FutureEvents.Infrastructure;
using Xunit;
using Xunit.Abstractions;

namespace GridDomain.Tests.Acceptance.FutureDomainEvents
{
    public class Given_aggregate_When_raising_several_future_events_Persistent : FutureEventsTest
    {
        public Given_aggregate_When_raising_several_future_events_Persistent(ITestOutputHelper output) : base(output) { }

        [Fact]
        public async Task FutureDomainEvent_envelops_has_unique_id()
        {
            var aggregateId = Guid.NewGuid().ToString();
            var testCommandA = new ScheduleEventInFutureCommand(DateTime.Now.AddSeconds(1), aggregateId, "test value A");

            var waitResult = await Node.Prepare(testCommandA)
                                       .Expect<FutureEventOccuredEvent>()
                                       .Execute();
            var eventA = waitResult.Message<FutureEventOccuredEvent>();

            var testCommandB = new ScheduleEventInFutureCommand(DateTime.Now.AddSeconds(1), aggregateId, "test value B");
            var result = await Node.Prepare(testCommandB)
                                   .Expect<FutureEventOccuredEvent>()
                                   .Execute();

            var eventB = result.Message<FutureEventOccuredEvent>();

            //Envelop_ids_are_different()
            Assert.NotEqual(eventA.FutureEventId, eventB.FutureEventId);
            //Envelop_id_not_equal_to_aggregate_id()
            Assert.True(eventA.Id != aggregateId && aggregateId != eventB.Id);
        }
    }
}