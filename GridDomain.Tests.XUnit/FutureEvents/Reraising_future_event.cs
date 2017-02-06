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
        public async Task  Given_aggregate_When_reraising_future_event_Then_it_fires_in_time()
        {
            var aggregateId = Guid.NewGuid();

            var testCommand = new ScheduleEventInFutureCommand(DateTime.Now.AddSeconds(1), aggregateId,"test value");

            await Node.Prepare(testCommand).Expect<TestDomainEvent>().Execute();

            var reraiseTime = DateTime.Now.AddSeconds(1);

            testCommand = new ScheduleEventInFutureCommand(reraiseTime, aggregateId, "test value");

            await Node.Prepare(testCommand).Expect<TestDomainEvent>().Execute();

            var aggregate = await this.LoadAggregate<TestAggregate>(aggregateId);

            Assert.True(reraiseTime.Second - aggregate.ProcessedTime.Second <=  1);
        }

        public Reraising_future_event(ITestOutputHelper output) : base(output) {}
    }
}