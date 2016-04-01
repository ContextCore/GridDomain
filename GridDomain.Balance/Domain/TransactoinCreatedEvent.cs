using System;
using GridDomain.EventSourcing;
using NMoneys;

namespace GridDomain.Balance.Domain
{
    public class TransactoinCreatedEvent:DomainEvent
    {
        public Guid BusinessId { get; set; }
        public Guid BalanceId { get; set; }
        public Money TransactionAmount { get; set; }
        public Guid TransactionSource { get; set; }
        public Guid TransactionId { get; set; }

        public TransactoinCreatedEvent(Guid sourceId) : base(sourceId)
        {
        }
    }
}