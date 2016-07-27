using System.Collections.Generic;
using GridDomain.EventSourcing.VersionedTypeSerialization;
using GridDomain.Tests.EventsUpgrade.Domain.Events;

namespace GridDomain.Tests.Acceptance.EventsUpgrade.SampleDomain
{
    class BalanceChangedDomainEventAdapter1 : DomainEventAdapter<BalanceChangedEvent_V0, BalanceChangedEvent_V1>
    {
        public override IEnumerable<BalanceChangedEvent_V1> ConvertEvent(BalanceChangedEvent_V0 evt)
        {
            yield return new BalanceChangedEvent_V1(evt.AmplifiedAmountChange, evt.SourceId);
        }
    }
}