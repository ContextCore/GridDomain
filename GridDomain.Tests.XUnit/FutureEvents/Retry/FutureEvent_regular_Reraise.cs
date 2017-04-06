using System;
using System.Threading.Tasks;
using GridDomain.CQRS;
using GridDomain.Node;
using GridDomain.Node.AkkaMessaging.Waiting;
using GridDomain.Scheduling.Integration;
using GridDomain.Scheduling.Quartz.Retry;
using GridDomain.Tests.XUnit.FutureEvents.Infrastructure;
using Xunit;
using Xunit.Abstractions;

namespace GridDomain.Tests.XUnit.FutureEvents.Retry
{
    public class FutureEvent_regular_Reraise : NodeTestKit
    {
        public FutureEvent_regular_Reraise(ITestOutputHelper output) : base(output, new ReraiseTestFixture()) {}

        private class ReraiseTestFixture : FutureEventsFixture
        {
            protected override NodeSettings CreateNodeSettings()
            {
                var nodeSettings = base.CreateNodeSettings();
                //Two fast retries
                nodeSettings.QuartzJobRetrySettings = new InMemoryRetrySettings(2,
                                                                                TimeSpan.FromMilliseconds(10),
                                                                                new DefaultExceptionPolicy());
                return nodeSettings;
            }
        }

        [Fact]
        public async Task Should_retry_on_exception()
        {
            //will retry 1 time
            var command = new ScheduleErrorInFutureCommand(DateTime.Now.AddSeconds(0.5), Guid.NewGuid(), "test value A", 1);

            var waiter = Node.Prepare(command)
                             .Expect<JobFailed>()
                             .And<JobSucceeded>()
                             .And<TestErrorDomainEvent>()
                             .Execute(TimeSpan.FromSeconds(100));

            var res = await waiter;
            var actual = res.Message<TestErrorDomainEvent>().Value;

            Assert.Equal(command.Value, actual);
        }
    }
}