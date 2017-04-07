using System;
using System.Threading.Tasks;
using GridDomain.Common;
using GridDomain.CQRS;
using GridDomain.Node;
using GridDomain.Node.AkkaMessaging.Waiting;
using GridDomain.Scheduling.Integration;
using GridDomain.Scheduling.Quartz.Retry;
using GridDomain.Tests.XUnit.CommandsExecution;
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
        public void Should_retry_on_exception()
        {
            //will retry 1 time
            var command = new ScheduleErrorInFutureCommand(DateTime.Now.AddSeconds(0.5), Guid.NewGuid(), "test value A", 1);

            //using testkit waiting to avoid exception from aggregate
            Node.Transport.Subscribe<MessageMetadataEnvelop<JobFailed>>(TestActor);
            Node.Transport.Subscribe<MessageMetadataEnvelop<JobSucceeded>>(TestActor);
            Node.Transport.Subscribe<MessageMetadataEnvelop<TestErrorDomainEvent>>(TestActor);

            Node.Execute(command);

            FishForMessage<MessageMetadataEnvelop<JobFailed>>(m => true, TimeSpan.FromSeconds(100));
            FishForMessage<MessageMetadataEnvelop<JobSucceeded>>(m => true);
            var res = FishForMessage<MessageMetadataEnvelop<TestErrorDomainEvent>>(m => true);

            Assert.Equal(command.Value, res.Message.Value);
        }

        [Fact]
        public async Task Should_forward_error_to_caller()
        {
            await Node.Prepare(new ScheduleErrorInFutureCommand(DateTime.Now.AddSeconds(0.5), Guid.NewGuid(), "test value A", 1))
                      .Expect<JobFailed>()
                      .Execute().ShouldThrow<TestScheduledException>();
        }
    }
}