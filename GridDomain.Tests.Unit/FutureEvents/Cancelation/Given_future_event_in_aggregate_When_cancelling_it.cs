using System;
using System.Threading;
using System.Threading.Tasks;
using GridDomain.CQRS;
using GridDomain.EventSourcing.FutureEvents;
using GridDomain.Node.Actors;
using GridDomain.Node.AkkaMessaging.Waiting;
using GridDomain.Tests.Unit.FutureEvents.Infrastructure;
using Microsoft.Practices.Unity;
using NUnit.Framework;
using Quartz;

namespace GridDomain.Tests.Unit.FutureEvents.Cancelation
{
    [TestFixture]

    public class Given_future_event_in_aggregate_When_cancelling_it : FutureEventsTest
    {
        private DateTime _scheduledTime;
        private FutureEventCanceledEvent _futureEventCancelation;
        private ScheduleEventInFutureCommand _testCommand;
        private FutureEventScheduledEvent _futureEventEnvelop;
        private CancelFutureEventCommand _cancelFutureEventCommand;


        public async Task When_raising_future_event()
        {
            _scheduledTime = DateTime.Now.AddSeconds(200);
            _testCommand = new ScheduleEventInFutureCommand(_scheduledTime, Guid.NewGuid(), "test value");

            _futureEventEnvelop = (await GridNode.Prepare(_testCommand)
                                                .Expect<FutureEventScheduledEvent>()
                                                .Execute())
                                  .Message<FutureEventScheduledEvent>();
                
            _cancelFutureEventCommand = new CancelFutureEventCommand(_testCommand.AggregateId, _testCommand.Value);

            _futureEventCancelation = (await GridNode.Prepare(_cancelFutureEventCommand)
                                                                   .Expect<FutureEventCanceledEvent>()
                                                                   .Execute())
                                                     .Message<FutureEventCanceledEvent>();
        }

        protected override TimeSpan Timeout => TimeSpan.FromSeconds(5);

        [Then]
        public async Task Cancelation_event_has_same_id_as_future_event()
        {
            await When_raising_future_event();
            Assert.AreEqual(_futureEventEnvelop.Id, _futureEventCancelation.FutureEventId);
        }
        
        [Then]
        public async Task Scheduler_does_not_contain_job_for_future_event()
        {
            await When_raising_future_event();
            var scheduler = GridNode.Container.Resolve<IScheduler>();
            //scheduler needs time to cancel the event
            //TODO: remove sleep to explicit wait
            Thread.Sleep(1000);
            var scheduleKey = AggregateActor<TestAggregate>.CreateScheduleKey(_futureEventEnvelop.Id,
                _testCommand.AggregateId, "");

            var jobKey = new JobKey(scheduleKey.Name, scheduleKey.Group);

            Assert.False(scheduler.CheckExists(jobKey));
        }

        public Given_future_event_in_aggregate_When_cancelling_it(bool inMemory) : base(inMemory)
        {
        }

        public Given_future_event_in_aggregate_When_cancelling_it() : base(true)
        {
        }
    }
}