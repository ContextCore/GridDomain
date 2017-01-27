using System;
using GridDomain.EventSourcing;

namespace GridDomain.Tests.XUnit.EventsUpgrade.Domain.Events
{
    public class BalanceChangedEvent_V1 : DomainEvent
    {
        public decimal AmountChange;
        public BalanceChangedEvent_V1(decimal amountChange, Guid sourceId, DateTime? createdTime = null, Guid sagaId = new Guid()) : base(sourceId, createdTime, sagaId)
        {
            AmountChange = amountChange;
        }
    }
}