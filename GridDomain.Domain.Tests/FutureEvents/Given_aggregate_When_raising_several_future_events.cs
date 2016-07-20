using System;
using System.Linq;
using GridDomain.EventSourcing.Sagas.FutureEvents;
using GridDomain.Node.FutureEvents;
using GridDomain.Tests.FutureEvents.Infrastructure;
using NUnit.Framework;

namespace GridDomain.Tests.FutureEvents
{
    [TestFixture]
    public class Given_aggregate_When_raising_several_future_events : FutureEventsTest_InMemory
    {
        private FutureDomainEvent _eventA;
        private FutureDomainEvent _eventB;
        private Guid _aggregateId;

        [TestFixtureSetUp]
        public void FutureDomainEvent_envelops_has_unique_id()
        {
            _aggregateId = Guid.NewGuid();
            var testCommandA = new RaiseEventInFutureCommand(DateTime.Now.AddSeconds(1), _aggregateId, "test value A");
            var testCommandB = new RaiseEventInFutureCommand(DateTime.Now.AddSeconds(2), _aggregateId, "test value B");
            _eventA = (FutureDomainEvent)ExecuteAndWaitFor<FutureDomainEvent>(testCommandA).Recieved.First();
            _eventB = (FutureDomainEvent)ExecuteAndWaitFor<FutureDomainEvent>(testCommandB).Recieved.First();
        }

        [Then]
        public void Envelop_ids_are_different()
        {
            Assert.AreNotEqual(_eventA.Id, _eventB.Id);
        }

        [Then]
        public void Envelop_id_not_equal_to_aggregate_id()
        {
            Assert.True(_eventA.Id != _aggregateId && _aggregateId !=  _eventB.Id);
        }
    }
}