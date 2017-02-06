using System;
using System.Threading.Tasks;
using GridDomain.CQRS;
using GridDomain.Tests.XUnit.FutureEvents.Infrastructure;
using Serilog.Events;
using Xunit;
using Xunit.Abstractions;

namespace GridDomain.Tests.XUnit.FutureEvents
{

    public class Raising_future_events_too_late : FutureEventsTest
    {
        protected async Task<TestAggregate> RaiseFutureEventInTime(DateTime scheduledTime)
        {
            var testCommand = new ScheduleEventInFutureCommand(scheduledTime, Guid.NewGuid(), "test value");

            await Node.Prepare(testCommand)
                      .Expect<TestDomainEvent>()
                      .Execute();

            return await this.LoadAggregate<TestAggregate>(testCommand.AggregateId);
        }

        [Fact]
        public async Task Given_aggregate_When_raising_future_event_in_past_Then_it_fires_immediatly()
        {
            var scheduledTime = DateTime.Now.AddSeconds(-5);
            var now = DateTime.Now;
            var aggregate = await RaiseFutureEventInTime(scheduledTime);
            Assert.True(now.Second - aggregate.ProcessedTime.Second <= 1);
        }

        public Raising_future_events_too_late(ITestOutputHelper output) : base(output) {}
    }
}