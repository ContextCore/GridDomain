using System;
using GridDomain.Tests.FutureEvents.Infrastructure;
using NUnit.Framework;

namespace GridDomain.Tests.FutureEvents
{
    [TestFixture]
    public class Reraising_future_event : FutureEventsTest_InMemory
    {
        protected override TimeSpan Timeout => TimeSpan.FromSeconds(20);

        [Test]
        public void Given_aggregate_When_reraising_future_event_Then_it_fires_in_time()
        {
            var aggregateId = Guid.NewGuid();

            var testCommand = new ScheduleEventInFutureCommand(DateTime.Now.AddSeconds(1), aggregateId,"test value");

            ExecuteAndWaitFor<TestDomainEvent>(testCommand);

            var reraiseTime = DateTime.Now.AddSeconds(1);

            testCommand = new ScheduleEventInFutureCommand(reraiseTime, aggregateId, "test value");

            ExecuteAndWaitFor<TestDomainEvent>(testCommand);

            var aggregate = LoadAggregate<TestAggregate>(aggregateId);

            Assert.LessOrEqual(reraiseTime.Second - aggregate.ProcessedTime.Second, 1);
        }
    }
}