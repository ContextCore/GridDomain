using System;
using System.Threading.Tasks;
using GridDomain.CQRS;
using GridDomain.Tests.Unit.FutureEvents.Infrastructure;
using Xunit;
using Xunit.Abstractions;

namespace GridDomain.Tests.Unit.FutureEvents
{
    public class Reraising_future_event : FutureEventsTest
    {
        public Reraising_future_event(ITestOutputHelper output) : base(output) {}

        [Fact]
        public async Task Given_aggregate_When_reraising_future_event_Then_it_fires_in_time()
        {
            var aggregateId = Guid.NewGuid().ToString();

            await
                Node.Prepare(new ScheduleEventInFutureCommand(DateTime.Now.AddSeconds(0.5), aggregateId, "test value"))
                    .Expect<ValueChangedSuccessfullyEvent>()
                    .Execute();

            var reraiseTime = DateTime.Now.AddSeconds(0.5);

            await
                Node.Prepare(new ScheduleEventInFutureCommand(reraiseTime, aggregateId, "test value"))
                    .Expect<ValueChangedSuccessfullyEvent>()
                    .Execute();

            var aggregate = await this.LoadAggregateByActor<TestFutureEventsAggregate>(aggregateId);

            Assert.True(reraiseTime.Second - aggregate.ProcessedTime.Second <= 1);
        }
    }
}