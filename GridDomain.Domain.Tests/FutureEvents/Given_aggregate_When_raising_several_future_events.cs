using System;
using GridDomain.EventSourcing.Sagas.FutureEvents;
using GridDomain.Node;
using GridDomain.Node.AkkaMessaging.Waiting;
using GridDomain.Tests.FutureEvents.Infrastructure;
using NUnit.Framework;

namespace GridDomain.Tests.FutureEvents
{
    [TestFixture]
    public class Given_aggregate_When_raising_several_future_events : FutureEventsTest
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

            _eventA = GridNode.Execute(testCommandA, Timeout, ExpectedMessage.Once<FutureEventOccuredEvent>());
            _eventB = GridNode.Execute(testCommandB, Timeout, ExpectedMessage.Once<FutureEventOccuredEvent>());
        }

        protected override TimeSpan Timeout => TimeSpan.FromSeconds(3);

        [Then]
        public void Envelop_ids_are_different()
        {
            Assert.AreNotEqual(_eventA.FutureEventId, _eventB.FutureEventId);
        }

        [Then]
        public void Envelop_id_not_equal_to_aggregate_id()
        {
            Assert.True(_eventA.Id != _aggregateId && _aggregateId !=  _eventB.Id);
        }

        public Given_aggregate_When_raising_several_future_events(bool inMemory) : base(inMemory)
        {
        }
        public Given_aggregate_When_raising_several_future_events() : base(true)
        {
        }
    }
}