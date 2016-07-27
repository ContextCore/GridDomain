using System.Collections.Generic;
using GridDomain.EventSourcing.VersionedTypeSerialization;
using GridDomain.Tests.EventsUpgrade.Domain;

namespace GridDomain.Tests.EventsUpgrade
{
    class EventUpdater2 : DomainEventAdapter<BalanceAggregate, TestEvent_V1, TestEvent_V2>
    {
        public override IEnumerable<TestEvent_V2> ConvertEvent(BalanceAggregate aggregate, TestEvent_V1 evt)
        {
            yield return new TestEvent_V2(evt.SourceId) { Field3 = evt.Field2 };
        }
    }
}