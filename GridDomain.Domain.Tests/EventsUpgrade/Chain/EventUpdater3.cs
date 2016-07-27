using System.Collections.Generic;
using GridDomain.EventSourcing.VersionedTypeSerialization;
using GridDomain.Tests.EventsUpgrade.Domain;

namespace GridDomain.Tests.EventsUpgrade
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