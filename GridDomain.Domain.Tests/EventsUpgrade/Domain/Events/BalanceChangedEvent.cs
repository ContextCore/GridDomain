using System;
using GridDomain.EventSourcing;

namespace GridDomain.Tests.EventsUpgrade.Domain.Events
{
    public class BalanceChangedEvent : DomainEvent
    {
        public decimal AmountChange;
        public decimal Koefficient;
        public decimal AmplifiedAmountChange;

        public BalanceChangedEvent(decimal amountChange,Guid sourceId, decimal koefficient = 1, DateTime? createdTime = null, Guid sagaId = new Guid()) : base(sourceId, createdTime, sagaId)
        {
            Koefficient = koefficient;
            AmountChange = amountChange;
            AmplifiedAmountChange = Koefficient*AmountChange;
        }
    }
}