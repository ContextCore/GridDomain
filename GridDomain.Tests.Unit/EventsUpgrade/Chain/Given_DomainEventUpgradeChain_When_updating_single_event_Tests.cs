using System;
using System.Linq;
using GridDomain.EventSourcing.Adapters;
using GridDomain.Tests.Unit.EventsUpgrade.Domain;
using GridDomain.Tests.Unit.EventsUpgrade.Events;
using Xunit;

namespace GridDomain.Tests.Unit.EventsUpgrade.Chain
{
    public class Given_DomainEventUpgradeChain_When_updating_single_event_Tests
    {
        private TestEvent_V2 _resultEvent;
        private TestEvent _initialEvent;

        [Fact]
        public void When_updating_single_event_Tests()
        {
            var chain = new EventsAdaptersCatalog();
            chain.Register(new DomainEventUpdater1());
            chain.Register(new DomainEventUpdater2());

            var balanceAggregate = new BalanceAggregate(Guid.NewGuid(), 10);
            _initialEvent = new TestEvent(balanceAggregate.Id);
            _resultEvent = chain.Update(_initialEvent).FirstOrDefault() as TestEvent_V2;
            // Event_type_was_updated()
            Assert.IsAssignableFrom<TestEvent_V2>(_resultEvent);
            //Event_id_was_remained()
            Assert.Equal(_initialEvent.SourceId, _resultEvent.SourceId);
            //Event_Field_was_remained()
            Assert.Equal(_initialEvent.Field, _resultEvent.Field3);
        }
    }
}