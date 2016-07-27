using System;
using System.Collections.Generic;
using System.Linq;
using GridDomain.EventSourcing.VersionedTypeSerialization;
using GridDomain.Tests.EventsUpgrade.Domain;
using NUnit.Framework;

namespace GridDomain.Tests.EventsUpgrade
{
    [TestFixture]
    public class Given_DomainEventUpgradeChain_When_updating_events_split_Tests
    {
        private IEnumerable<TestEvent_V3> _resultEvent;
        private TestEvent_V2 _initialEvent;

        [TestFixtureSetUp]
        public void When_updating_single_event_Tests()
        {
            var chain = new DomainEventsUpgradeChain<BalanceAggregate>();
            chain.Register(new EventUpdater3());
            var balanceAggregate = new BalanceAggregate(Guid.NewGuid(), 10);
            _initialEvent = new TestEvent_V2(balanceAggregate.Id);
            _resultEvent = chain.Update(balanceAggregate, _initialEvent).OfType<TestEvent_V3>();
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