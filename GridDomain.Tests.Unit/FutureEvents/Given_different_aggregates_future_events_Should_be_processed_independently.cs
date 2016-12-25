using System;
using System.Threading.Tasks;
using GridDomain.CQRS;
using GridDomain.EventSourcing.FutureEvents;
using GridDomain.Tests.Unit.FutureEvents.Infrastructure;
using NUnit.Framework;

namespace GridDomain.Tests.Unit.FutureEvents
{
    [TestFixture]
    public class Given_different_aggregates_future_events_Should_be_processed_independently : FutureEventsTest_InMemory
    {
        private FutureEventOccuredEvent _eventA;
        private FutureEventOccuredEvent _eventB;
        private ScheduleEventInFutureCommand _commandA;
        private ScheduleEventInFutureCommand _commandB;

        [OneTimeSetUp]
        public void Raising_several_future_events_for_different_aggregates()
        {
            _commandA = new ScheduleEventInFutureCommand(DateTime.Now.AddSeconds(0.5), Guid.NewGuid(), "test value A");
            var expectedMessageA = Expect.Message<FutureEventOccuredEvent>(e => e.SourceId, _commandA.AggregateId);
            var planA = CommandPlan.New(_commandA, expectedMessageA);

            _commandB = new ScheduleEventInFutureCommand(DateTime.Now.AddSeconds(0.5), Guid.NewGuid(), "test value B");
            var expectedMessageB = Expect.Message<FutureEventOccuredEvent>(e => e.SourceId, _commandB.AggregateId);
            var planB = CommandPlan.New(_commandB, expectedMessageB);

            var taskA = GridNode.Execute<FutureEventOccuredEvent>(planA).ContinueWith( r => _eventA = r.Result);
            var taskB = GridNode.Execute<FutureEventOccuredEvent>(planB).ContinueWith( r=>  _eventB = r.Result);

            Task.WaitAll(taskA, taskB);
        }

        [Then]
        public void Future_event_ids_are_different()
        {
            Assert.AreNotEqual(_eventA.FutureEventId, _eventB.FutureEventId);
        }

        [Then]
        public void EventA_is_result_of_commandA()
        {
            Assert.AreEqual(_eventA.SourceId, _commandA.AggregateId);
        }

        [Then]
        public void EventB_is_result_of_commandB()
        {
            Assert.AreEqual(_eventB.SourceId, _commandB.AggregateId);
        }
    }
}