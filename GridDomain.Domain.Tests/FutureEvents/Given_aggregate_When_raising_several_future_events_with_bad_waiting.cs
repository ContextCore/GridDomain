using System;
using System.Linq;
using GridDomain.EventSourcing.Sagas.FutureEvents;
using GridDomain.Tests.FutureEvents.Infrastructure;
using NUnit.Framework;

namespace GridDomain.Tests.FutureEvents
{
    [TestFixture]
    [Ignore("example to be fixed")]
    public class Given_aggregate_When_raising_several_future_events_with_bad_waiting : FutureEventsTest_InMemory
    {
        private FutureEventOccuredEvent _eventA;
        private FutureEventOccuredEvent _eventB;
        private Guid _aggregateId;

        [TestFixtureSetUp]
        public void FutureDomainEvent_envelops_has_unique_id()
        {
            _aggregateId = Guid.NewGuid();
            var testCommandA = new RaiseEventInFutureCommand(DateTime.Now.AddSeconds(1), _aggregateId, "test value A");
            var testCommandB = new RaiseEventInFutureCommand(DateTime.Now.AddSeconds(2), _aggregateId, "test value B");
            _eventA = (FutureEventOccuredEvent)ExecuteAndWaitFor<FutureEventOccuredEvent>(testCommandA).Recieved.First();
            _eventB = (FutureEventOccuredEvent)ExecuteAndWaitFor<FutureEventOccuredEvent>(testCommandB).Recieved.First();

            //_eventA and _eventB are same instance!
        }

        //Failing!!!
        [Then]
        public void Envelop_ids_are_not_equal()
        {
            Assert.AreNotEqual(_eventA.FutureEventId, _eventB.FutureEventId);
        }
    }
}