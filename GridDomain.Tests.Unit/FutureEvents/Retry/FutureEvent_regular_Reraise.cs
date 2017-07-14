using System;
using System.Threading.Tasks;
using GridDomain.Common;
using GridDomain.CQRS;
using GridDomain.Node;
using GridDomain.Scheduling;
using GridDomain.Scheduling.Akka.Messages;
using GridDomain.Scheduling.Quartz;
using GridDomain.Scheduling.Quartz.Retry;
using GridDomain.Tests.Common;
using GridDomain.Tests.Unit.DependencyInjection.FutureEvents.Infrastructure;
using GridDomain.Tests.Unit.FutureEvents.Infrastructure;
using Xunit;
using Xunit.Abstractions;
using Akka.Actor;

namespace GridDomain.Tests.Unit.DependencyInjection.FutureEvents.Retry
{
    public class FutureEvent_regular_Reraise : NodeTestKit
    {
        public FutureEvent_regular_Reraise(ITestOutputHelper output) : base(output, new FastReraiseTestFixture()) { }

        private class FastReraiseTestFixture : FutureEventsFixture
        {
            protected override NodeSettings CreateNodeSettings()
            {
                var nodeSettings = base.CreateNodeSettings();
                //Two fast retries
                nodeSettings.QuartzConfig.RetryOptions = new InMemoryRetrySettings(2,
                                                                                   TimeSpan.FromMilliseconds(10),
                                                                                   new AlwaysRetryExceptionPolicy());
                return nodeSettings;
            }
        }

        [Fact]
        public void Should_retry_on_exception()
        {
            var scheduler = Node.System.GetExtension<SchedulingExtension>().SchedulingActor;
            var command = new BoomNowCommand(Guid.NewGuid());
            var executionOptions = new ExecutionOptions(DateTime.UtcNow.AddMilliseconds(100),
                                                        typeof(ValueChangedSuccessfullyEvent),command.AggregateId);

            var scheduleCommandExecution = new ScheduleCommandExecution(command, new ScheduleKey(command.Id,"test","test"), executionOptions );
            scheduler.Tell(scheduleCommandExecution);
            Node.Transport.Subscribe<MessageMetadataEnvelop<JobFailed>>(TestActor);

            //job will be retried one time, but aggregate will fail permanently due to error on apply method
            FishForMessage<MessageMetadataEnvelop<JobFailed>>(m => true);
            FishForMessage<MessageMetadataEnvelop<JobFailed>>(m => true);
        }
    }
}