using System;
using Akka.Actor;
using GridDomain.Common;
using GridDomain.Node;
using GridDomain.Scheduling;
using GridDomain.Scheduling.Akka;
using GridDomain.Scheduling.Akka.Messages;
using GridDomain.Scheduling.Quartz;
using GridDomain.Scheduling.Quartz.Retry;
using GridDomain.Tests.Unit.FutureEvents.Infrastructure;
using Xunit;
using Xunit.Abstractions;

namespace GridDomain.Tests.Unit.FutureEvents.Retry
{
    public class FutureEvent_regular_Reraise : NodeTestKit
    {
        public FutureEvent_regular_Reraise(ITestOutputHelper output) : base( 
            new FutureEventsFixture(output,new InMemoryRetrySettings(2,
                                                             TimeSpan.FromMilliseconds(10),
                                                             new AlwaysRetryExceptionPolicy()))) { }


        [Fact]
        public void Should_retry_on_exception()
        {
            var scheduler = Node.System.GetExtension<SchedulingExtension>()
                                .SchedulingActor;
            var command = new BoomNowCommand(Guid.NewGuid().ToString());
            var executionOptions = new ExecutionOptions(DateTime.UtcNow.AddMilliseconds(100),
                                                        typeof(ValueChangedSuccessfullyEvent),
                                                        command.AggregateId);

            var scheduleCommandExecution = new ScheduleCommandExecution(command, new ScheduleKey("test", "test"), executionOptions);
            scheduler.Tell(scheduleCommandExecution);
            Node.Transport.Subscribe<MessageMetadataEnvelop>(TestActor);

            //job will be retried one time, but aggregate will fail permanently due to error on apply method
            FishForMessage<MessageMetadataEnvelop>(m => m.Message is JobFailed, TimeSpan.FromSeconds(10));
            FishForMessage<MessageMetadataEnvelop>(m => m.Message is JobFailed, TimeSpan.FromSeconds(10));
        }
    }
}