using System.Collections.Generic;
using GridDomain.EventSourcing.Adapters;
using GridDomain.Tests.XUnit.EventsUpgrade.Events;

namespace GridDomain.Tests.XUnit.EventsUpgrade.Chain
{
    internal class DomainEventUpdater1 : DomainEventAdapter<TestEvent, TestEvent_V1>
    {
        public override IEnumerable<TestEvent_V1> ConvertEvent(TestEvent evt)
        {
            yield return new TestEvent_V1(evt.SourceId) {Field2 = evt.Field};
        }
    }
}