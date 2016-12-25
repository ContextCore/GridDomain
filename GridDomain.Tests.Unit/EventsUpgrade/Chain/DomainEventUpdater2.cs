using System.Collections.Generic;
using GridDomain.EventSourcing.Adapters;
using GridDomain.Tests.Unit.EventsUpgrade.Events;

namespace GridDomain.Tests.Unit.EventsUpgrade.Chain
{
    class DomainEventUpdater2 : DomainEventAdapter<TestEvent_V1, TestEvent_V2>
    {
        public override IEnumerable<TestEvent_V2> ConvertEvent(TestEvent_V1 evt)
        {
            yield return new TestEvent_V2(evt.SourceId) { Field3 = evt.Field2 };
        }
    }
}