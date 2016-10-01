using System;
using System.Collections.Generic;
using System.Linq;
using GridDomain.EventSourcing.Adapters;
using GridDomain.Tests.EventsUpgrade.Domain;
using GridDomain.Tests.EventsUpgrade.Events;
using NUnit.Framework;

namespace GridDomain.Tests.EventsUpgrade.Chain
{
    [TestFixture]
    public class Given_DomainEventUpgradeChain_When_updating_events_split_Tests
    {
        private IEnumerable<TestEvent_V3> _resultEvent;
        private TestEvent_V2 _initialEvent;

        [OneTimeSetUp]
        public void When_updating_single_event_Tests()
        {
            var chain = new EventAdaptersCatalog();
            chain.Register(new DomainEventUpdater3());
            var balanceAggregate = new BalanceAggregate(Guid.NewGuid(), 10);
            _initialEvent = new TestEvent_V2(balanceAggregate.Id);
            _resultEvent = chain.Update(_initialEvent).OfType<TestEvent_V3>();
        }

        [Then]
        public void Events_type_was_updated()
        {
            Assert.IsInstanceOf<TestEvent_V3>(_resultEvent.FirstOrDefault());
        }

        [Then]
        public void All_events_were_produced()
        {
            Assert.AreEqual(2, _resultEvent.Count());
        }

        [Then]
        public void Event_id_was_remained()
        {
            Assert.True(_resultEvent.All(e => e.SourceId == _initialEvent.SourceId));
        }
    }
}