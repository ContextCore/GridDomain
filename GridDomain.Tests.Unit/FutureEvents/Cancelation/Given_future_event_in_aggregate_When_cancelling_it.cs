using System;
using System.Threading.Tasks;
using GridDomain.CQRS;
using GridDomain.Node.AkkaMessaging.Waiting;
using GridDomain.Scheduling;
using GridDomain.Scheduling.Akka;
using GridDomain.Tests.Unit.FutureEvents.Infrastructure;
using Quartz;
using Xunit;
using Xunit.Abstractions;

namespace GridDomain.Tests.Unit.FutureEvents.Cancelation
{
    public class Given_future_event_in_aggregate_When_cancelling_it : NodeTestKit
    {
        public Given_future_event_in_aggregate_When_cancelling_it(ITestOutputHelper output)
            : base(output, new FutureEventsFixture()) {}

        protected Given_future_event_in_aggregate_When_cancelling_it(ITestOutputHelper output, NodeTestFixture fixture)
            : base(output, fixture) {}

        [Fact]
        public async Task When_raising_future_event()
        {
            var scheduledTime = DateTime.Now.AddSeconds(200);
            var testCommand = new ScheduleEventInFutureCommand(scheduledTime, Guid.NewGuid(), "test value");

            var futureEventEnvelop =
                (await Node.Prepare(testCommand).Expect<FutureEventScheduledEvent>().Execute())
                .Message<FutureEventScheduledEvent>();

            var cancelFutureEventCommand = new CancelFutureEventCommand(testCommand.AggregateId, testCommand.Value);

            var futureEventCancelation =
                (await Node.Prepare(cancelFutureEventCommand).Expect<FutureEventCanceledEvent>().Execute())
                .Message<FutureEventCanceledEvent>();

            //Cancelation_event_has_same_id_as_future_event()
            Assert.Equal(futureEventEnvelop.Id, futureEventCancelation.FutureEventId);
            //Scheduler_does_not_contain_job_for_future_event()
            var scheduler = Node.System.GetExtension<SchedulingExtension>().Scheduler;

            //scheduler needs time to cancel the event
            //TODO: remove sleep to explicit wait
            await Task.Delay(2000);
            var scheduleKey = FutureEventsSchedulingMessageHandler.CreateScheduleKey(futureEventEnvelop.Id,
                                                                                     testCommand.AggregateId,
                                                                                     nameof(TestFutureEventsAggregate));

            var jobKey = new JobKey(scheduleKey.Name, scheduleKey.Group);

            Assert.False(scheduler.CheckExists(jobKey));
        }
    }
}