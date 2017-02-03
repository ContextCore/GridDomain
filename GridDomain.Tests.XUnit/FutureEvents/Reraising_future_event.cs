using System;
using System.Threading.Tasks;
using GridDomain.CQRS;
using GridDomain.Tests.XUnit.FutureEvents.Infrastructure;

namespace GridDomain.Tests.XUnit.FutureEvents
{
    
    public class Reraising_future_event : FutureEventsTest_InMemory
    {
        protected override TimeSpan DefaultTimeout => TimeSpan.FromSeconds(20);

        [Fact]
        public async Task  Given_aggregate_When_reraising_future_event_Then_it_fires_in_time()
        {
            var aggregateId = Guid.NewGuid();

            var testCommand = new ScheduleEventInFutureCommand(DateTime.Now.AddSeconds(1), aggregateId,"test value");

            await GridNode.Prepare(testCommand).Expect<TestDomainEvent>().Execute();

            var reraiseTime = DateTime.Now.AddSeconds(1);

            testCommand = new ScheduleEventInFutureCommand(reraiseTime, aggregateId, "test value");

            await GridNode.Prepare(testCommand).Expect<TestDomainEvent>().Execute();

            var aggregate = LoadAggregate<TestAggregate>(aggregateId);

            Assert.LessOrEqual(reraiseTime.Second - aggregate.ProcessedTime.Second, 1);
        }
    }
}