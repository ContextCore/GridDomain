using System;
using GridDomain.EventSourcing;

namespace GridDomain.Tests.Unit.EventsUpgrade.Domain.Events
{
    public class BalanceChangedEvent_V1 : DomainEvent
    {
        public BalanceChangedEvent_V1(decimal amountChange, string sourceId, DateTime? createdTime = null, string processId = null)
            : base(sourceId, processId: processId, createdTime: createdTime)
        {
            AmountChange = amountChange;
        }

        public decimal AmountChange { get; }
    }
}