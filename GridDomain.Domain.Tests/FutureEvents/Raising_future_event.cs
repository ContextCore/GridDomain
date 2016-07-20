using System;
using GridDomain.Tests.FutureEvents.Infrastructure;
using NUnit.Framework;

namespace GridDomain.Tests.FutureEvents
{
    [TestFixture]

    public class Raising_future_event : FutureEventsTest_InMemory
    {
        [Test]
        public void Given_aggregate_When_raising_future_event_Then_it_fires_in_time()
        {
            var scheduledTime = DateTime.Now.AddSeconds(1);
            var aggregate = RaiseFutureEventInTime(scheduledTime);
            Assert.LessOrEqual(scheduledTime.Second - aggregate.ProcessedTime.Second,1);
        }
    }

    [TestFixture]
    public class Reraising_future_event : FutureEventsTest_InMemory
    {
        protected override TimeSpan Timeout => TimeSpan.FromMinutes(10);

        [Test]
        public void Given_aggregate_When_reraising_future_event_Then_it_fires_in_time()
        {
            var aggregateId = Guid.NewGuid();

            var testCommand = new TestCommand(DateTime.Now.AddSeconds(1), aggregateId);

            ExecuteAndWaitFor<TestDomainEvent>(testCommand);

            var reraiseTime = DateTime.Now.AddSeconds(1);

            testCommand = new TestCommand(reraiseTime, aggregateId);

            ExecuteAndWaitFor<TestDomainEvent>(testCommand);

            var aggregate = LoadAggregate<TestAggregate>(aggregateId);

            Assert.LessOrEqual(reraiseTime.Second - aggregate.ProcessedTime.Second, 1);
        }
    }
}