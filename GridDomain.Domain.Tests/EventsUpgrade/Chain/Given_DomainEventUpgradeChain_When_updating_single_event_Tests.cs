using System;
using System.Linq;
using GridDomain.EventSourcing.Adapters;
using GridDomain.Tests.EventsUpgrade.Domain;
using GridDomain.Tests.EventsUpgrade.Events;
using NUnit.Framework;

namespace GridDomain.Tests.EventsUpgrade.Chain
{
    [TestFixture]
    public class Given_DomainEventUpgradeChain_When_updating_single_event_Tests
    {
        private TestEvent_V2 _resultEvent;
        private TestEvent _initialEvent;

        [OneTimeSetUp]
        public void When_updating_single_event_Tests()
        {
            var chain = new EventsAdaptersCatalog();
            chain.Register(new DomainEventUpdater1());
            chain.Register(new DomainEventUpdater2());

            var balanceAggregate = new BalanceAggregate(Guid.NewGuid(), 10);
            _initialEvent = new TestEvent(balanceAggregate.Id);
            _resultEvent = chain.Update(_initialEvent).FirstOrDefault() as TestEvent_V2;
        }

        [Then]
        public void Event_type_was_updated()
        {
            Assert.IsInstanceOf<TestEvent_V2>(_resultEvent);
        }

        [Then]
        public void Event_id_was_remained()
        {
            Assert.AreEqual(_initialEvent.SourceId,_resultEvent.SourceId);
        }

        [Then]
        public void Event_Field_was_remained()
        {
            Assert.AreEqual(_initialEvent.Field,_resultEvent.Field3);
        }
    }
}