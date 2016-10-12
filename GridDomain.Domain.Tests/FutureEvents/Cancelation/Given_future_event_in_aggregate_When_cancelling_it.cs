using System;
using System.Linq;
using System.Threading;
using GridDomain.EventSourcing.Sagas.FutureEvents;
using GridDomain.Node.Actors;
using GridDomain.Tests.FutureEvents.Infrastructure;
using Microsoft.Practices.Unity;
using NUnit.Framework;
using Quartz;

namespace GridDomain.Tests.FutureEvents.Cancelation
{
    [TestFixture]

    public class Given_future_event_in_aggregate_When_cancelling_it : FutureEventsTest
    {
        private DateTime _scheduledTime;
        private FutureEventCanceledEvent _futureEventCancelation;
        private RaiseEventInFutureCommand _testCommand;
        private FutureEventScheduledEvent _futureEventEnvelop;
        private CancelFutureEventCommand _cancelFutureEventCommand;

        [OneTimeSetUp]

        public void When_raising_future_event()
        {
            _scheduledTime = DateTime.Now.AddSeconds(200);
            _testCommand = new RaiseEventInFutureCommand(_scheduledTime, Guid.NewGuid(), "test value");

            _futureEventEnvelop = (FutureEventScheduledEvent)ExecuteAndWaitFor<FutureEventScheduledEvent>(_testCommand).Received.First();

            _cancelFutureEventCommand = new CancelFutureEventCommand(_testCommand.AggregateId, _testCommand.Value);

            _futureEventCancelation = (FutureEventCanceledEvent)ExecuteAndWaitFor<FutureEventCanceledEvent>(_cancelFutureEventCommand).Received.First();
        }

        protected override TimeSpan Timeout => TimeSpan.FromSeconds(5);

        [Then]
        public void Cancelation_event_has_same_id_as_future_event()
        {
            Assert.AreEqual(_futureEventEnvelop.Id, _futureEventCancelation.FutureEventId);
        }
        
        [Then]
        public void Scheduler_does_not_contain_job_for_future_event()
        {
            var scheduler = GridNode.Container.Resolve<IScheduler>();
            //scheduler needs time to cancel the event
            //TODO: remove sleep to explicit wait
            Thread.Sleep(200);
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