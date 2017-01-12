using System;
using System.Threading.Tasks;
using GridDomain.CQRS;
using GridDomain.EventSourcing.FutureEvents;
using GridDomain.Node.AkkaMessaging.Waiting;
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
        public async Task Raising_several_future_events_for_different_aggregates()
        {
            _commandA = new ScheduleEventInFutureCommand(DateTime.Now.AddSeconds(0.5), Guid.NewGuid(), "test value A");
            _commandB = new ScheduleEventInFutureCommand(DateTime.Now.AddSeconds(0.5), Guid.NewGuid(), "test value B");
         
            _eventA = (await GridNode.PrepareCommand(_commandA)
                                   .Expect<FutureEventOccuredEvent>()
                                   .Execute())
                                   .Message<FutureEventOccuredEvent>();

            _eventB = (await GridNode.PrepareCommand(_commandB)
                                     .Expect<FutureEventOccuredEvent>()
                                     .Execute())
                                     .Message<FutureEventOccuredEvent>();
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