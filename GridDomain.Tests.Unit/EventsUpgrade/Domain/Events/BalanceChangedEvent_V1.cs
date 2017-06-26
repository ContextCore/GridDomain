using System;
using GridDomain.EventSourcing;

namespace GridDomain.Tests.Unit.EventsUpgrade.Domain.Events
{
    public class BalanceChangedEvent_V1 : DomainEvent
    {
        public BalanceChangedEvent_V1(decimal amountChange, Guid sourceId, DateTime? createdTime = null, Guid? sagaId = null)
            : base(sourceId, sagaId: sagaId, createdTime: createdTime)
        {
            AmountChange = amountChange;
        }

        public decimal AmountChange { get; }
    }
}