using System;
using System.Threading.Tasks;
using GridDomain.Common;
using GridDomain.CQRS;
using GridDomain.Node;
using GridDomain.Scheduling.Quartz;
using GridDomain.Scheduling.Quartz.Retry;
using GridDomain.Tests.Common;
using GridDomain.Tests.Unit.DependencyInjection.FutureEvents.Infrastructure;
using Xunit;
using Xunit.Abstractions;

namespace GridDomain.Tests.Unit.DependencyInjection.FutureEvents.Retry
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
                nodeSettings.QuartzConfig.RetryOptions = new InMemoryRetrySettings(1,
                                                                                TimeSpan.FromMilliseconds(10),
                                                                                new DefaultExceptionPolicy());
                return nodeSettings;
            }
        }

        [Fact]
        public void Should_retry_on_exception()
        {
            //will retry 1 time
            var command = new ScheduleErrorInFutureCommand(DateTime.Now.AddSeconds(0.5), Guid.NewGuid(), "test value A", 1);

            //using testkit waiting to avoid exception from aggregate
            Node.Transport.Subscribe<MessageMetadataEnvelop<JobFailed>>(TestActor);

            Node.Execute(command);

            //job will be retried one time, but aggregate will fail permanently due to error on apply method
            FishForMessage<MessageMetadataEnvelop<JobFailed>>(m => true);
            FishForMessage<MessageMetadataEnvelop<JobFailed>>(m => true);
        }

       [Fact(Skip = "will enable later if need")]
       public async Task Should_forward_error_to_caller()
       {
           await Node.Prepare(new ScheduleErrorInFutureCommand(DateTime.Now.AddSeconds(0.1), Guid.NewGuid(), "test value A", 1))
                     .Expect<JobFailed>()
                     .Execute()
                     .ShouldThrow<TestScheduledException>();
       }
    }
}