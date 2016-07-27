using System.Collections.Generic;
using GridDomain.EventSourcing.VersionedTypeSerialization;
using GridDomain.Tests.EventsUpgrade.Domain;

namespace GridDomain.Tests.EventsUpgrade
{
    class EventUpdater1 : DomainEventAdapter<TestEvent, TestEvent_V1>
    {
        public override IEnumerable<TestEvent_V1> ConvertEvent(TestEvent evt)
        {
            yield return new TestEvent_V1(evt.SourceId) { Field2 = evt.Field };
        }
    }
}