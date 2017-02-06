using System;
using System.Threading.Tasks;
using GridDomain.CQRS;
using GridDomain.Tests.XUnit.FutureEvents.Infrastructure;
using Xunit;
using Xunit.Abstractions;

namespace GridDomain.Tests.XUnit.FutureEvents
{
    public class Reraising_future_event : FutureEventsTest
    {
        [Fact]
        public async Task Given_aggregate_When_reraising_future_event_Then_it_fires_in_time()
        {
            var aggregateId = Guid.NewGuid();

            await Node.Prepare(new ScheduleEventInFutureCommand(DateTime.Now.AddSeconds(0.5), aggregateId, "test value"))
                      .Expect<TestDomainEvent>()
                      .Execute();

            var reraiseTime = DateTime.Now.AddSeconds(0.5);

            await Node.Prepare(new ScheduleEventInFutureCommand(reraiseTime, aggregateId, "test value"))
                      .Expect<TestDomainEvent>()
                      .Execute();

            var aggregate = await this.LoadAggregate<FutureEventsAggregate>(aggregateId);

            Assert.True(reraiseTime.Second - aggregate.ProcessedTime.Second <= 1);
        }

        public Reraising_future_event(ITestOutputHelper output) : base(output) {}
    }
}