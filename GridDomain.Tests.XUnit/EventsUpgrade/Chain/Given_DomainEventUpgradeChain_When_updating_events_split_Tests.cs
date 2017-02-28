using System;
using System.Collections.Generic;
using System.Linq;
using GridDomain.EventSourcing.Adapters;
using GridDomain.Tests.XUnit.EventsUpgrade.Domain;
using GridDomain.Tests.XUnit.EventsUpgrade.Events;
using Xunit;

namespace GridDomain.Tests.XUnit.EventsUpgrade.Chain
{
    public class Given_DomainEventUpgradeChain_When_updating_events_split_Tests
    {
        private IEnumerable<TestEvent_V3> _resultEvent;
        private TestEvent_V2 _initialEvent;

        [Fact]
        public void When_updating_single_event_Tests()
        {
            var chain = new EventsAdaptersCatalog();
            chain.Register(new DomainEventUpdater3());
            var balanceAggregate = new BalanceAggregate(Guid.NewGuid(), 10);
            _initialEvent = new TestEvent_V2(balanceAggregate.Id);
            _resultEvent = chain.Update(_initialEvent)
                                .OfType<TestEvent_V3>();

            //Events_type_was_updated()
            Assert.IsAssignableFrom<TestEvent_V3>(_resultEvent.FirstOrDefault());
            //All_events_were_produced()
            Assert.Equal(2, _resultEvent.Count());
            //Event_id_was_remained()
            Assert.True(_resultEvent.All(e => e.SourceId == _initialEvent.SourceId));
        }
    }
}