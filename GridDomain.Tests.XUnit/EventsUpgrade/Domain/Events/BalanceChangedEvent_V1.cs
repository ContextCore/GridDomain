using System;
using GridDomain.EventSourcing;

namespace GridDomain.Tests.XUnit.EventsUpgrade.Domain.Events
{
    public class BalanceChangedEvent_V1 : DomainEvent
    {
        public decimal AmountChange { get;}
        public BalanceChangedEvent_V1(decimal amountChange, Guid sourceId, DateTime? createdTime = null, Guid? sagaId = null) : base(sourceId, createdTime, sagaId)
        {
            AmountChange = amountChange;
        }
    }
}