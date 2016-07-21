using System;
using System.Linq;
using GridDomain.EventSourcing.Sagas.FutureEvents;
using GridDomain.Node.Actors;
using GridDomain.Tests.FutureEvents.Infrastructure;
using Microsoft.Practices.Unity;
using NUnit.Framework;
using Quartz;

namespace GridDomain.Tests.FutureEvents
{
    [TestFixture]

    public class Given_future_event_in_aggregate_When_cancelling_it : FutureEventsTest_InMemory
    {
        private TestAggregate _aggregate;
        private DateTime _scheduledTime;
        private FutureEventCanceledEvent _futureEventCancelation;
        private RaiseEventInFutureCommand _testCommand;
        private FutureEventScheduledEvent _futureEventEnvelop;
        private CancelFutureEventCommand _cancelFutureEventCommand;

        [TestFixtureSetUp]

        public void When_raising_future_event()
        {
            _scheduledTime = DateTime.Now.AddSeconds(200);
            _testCommand = new RaiseEventInFutureCommand(_scheduledTime, Guid.NewGuid(), "test value");

            _futureEventEnvelop = (FutureEventScheduledEvent)ExecuteAndWaitFor<FutureEventScheduledEvent>(_testCommand).Recieved.First();

            _cancelFutureEventCommand = new CancelFutureEventCommand(_testCommand.AggregateId, _testCommand.Value);

            _futureEventCancelation = (FutureEventCanceledEvent)ExecuteAndWaitFor<FutureEventCanceledEvent>(_testCommand).Recieved.First();
        }

        [Then]
        public void Cancelation_event_has_same_id_as_future_event()
        {
            Assert.AreEqual(_futureEventEnvelop.Id, _futureEventCancelation.FutureEventId);
        }
        
        [Then]
        public void Scheduler_does_not_contain_job_for_future_event()
        {
            var scheduler = GridNode.Container.Resolve<IScheduler>();
            var scheduleKey = AggregateActor<TestAggregate>.CreateScheduleKey(_futureEventEnvelop.Id,
                _testCommand.AggregateId, "");

            var jobKey = new JobKey(scheduleKey.Name, scheduleKey.Group);

            Assert.False(scheduler.CheckExists(jobKey));
        }
    }
}