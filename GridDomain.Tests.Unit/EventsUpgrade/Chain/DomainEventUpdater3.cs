using System.Collections.Generic;
using GridDomain.EventSourcing.Adapters;
using GridDomain.Tests.Unit.EventsUpgrade.Events;

namespace GridDomain.Tests.Unit.EventsUpgrade.Chain
{
    class DomainEventUpdater3 : DomainEventAdapter<TestEvent_V2, TestEvent_V3>
    {
        public override IEnumerable<TestEvent_V3> ConvertEvent(TestEvent_V2 evt)
        {
            yield return new TestEvent_V3(evt.SourceId) { Field4 = evt.Field3 };
            yield return new TestEvent_V3(evt.SourceId) { Field4 = evt.Field3 };
        }
    }
}