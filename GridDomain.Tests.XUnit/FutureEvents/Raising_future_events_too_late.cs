using System;
using System.Threading.Tasks;
using GridDomain.CQRS;
using GridDomain.Tests.XUnit.FutureEvents.Infrastructure;
using Serilog.Events;
using Xunit;
using Xunit.Abstractions;

namespace GridDomain.Tests.XUnit.FutureEvents
{
    [Collection("FutureEvents")]
    public class Raising_future_events_too_late : FutureEventsTest
    {
        [Fact]
        public async Task Given_aggregate_When_raising_future_event_in_past_Then_it_fires_immediatly()
        {
            var scheduledTime = DateTime.Now.AddSeconds(-5);
            var now = DateTime.Now;
            var testCommand = new ScheduleEventInFutureCommand(scheduledTime, Guid.NewGuid(), "test value");

            await Node.Prepare(testCommand)
                      .Expect<TestDomainEvent>()
                      .Execute();

            var aggregate = await this.LoadAggregate<FutureEventsAggregate>(testCommand.AggregateId);

            Assert.True(now.Second - aggregate.ProcessedTime.Second <= 1);
        }

        public Raising_future_events_too_late(ITestOutputHelper output) : base(output) {}
    }
}