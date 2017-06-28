using System;
using System.Threading.Tasks;
using GridDomain.CQRS;
using GridDomain.Scheduling.Integration;
using GridDomain.Tests.Common;
using GridDomain.Tests.Unit.DependencyInjection.FutureEvents.Infrastructure;
using Xunit;
using Xunit.Abstractions;

namespace GridDomain.Tests.Unit.DependencyInjection.FutureEvents
{
    public class Raising_future_events_too_late : NodeTestKit
    {
        public Raising_future_events_too_late(ITestOutputHelper output) : base(output, new FutureEventsFixture()) {}
        protected Raising_future_events_too_late(ITestOutputHelper output, NodeTestFixture fixture) : base(output, fixture) {}

        [Fact]
        public async Task Given_aggregate_When_raising_future_event_in_past_Then_it_fires_immediatly()
        {
            var scheduledTime = DateTime.Now.AddSeconds(-5);
            var now = DateTime.Now;
            var testCommand = new ScheduleEventInFutureCommand(scheduledTime, Guid.NewGuid(), "test value");

            await Node.Prepare(testCommand)
                      .Expect<JobSucceeded>()
                      .And<JobSucceeded>()
                      .Execute();

            var aggregate = await Node.LoadAggregate<FutureEventsAggregate>(testCommand.AggregateId);

            Assert.True(now.Second - aggregate.ProcessedTime.Second <= 1);
        }
    }
}