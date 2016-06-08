using System;
using GridDomain.EventSourcing;
using NMoneys;

namespace BusinessNews.Domain
{
    public class TransactoinCreatedEvent : DomainEvent
    {
        public TransactoinCreatedEvent(Guid sourceId) : base(sourceId)
        {
        }

        public Guid BusinessId { get; set; }
        public Guid BalanceId { get; set; }
        public Money TransactionAmount { get; set; }
        public Guid TransactionSource { get; set; }
        public Guid TransactionId { get; set; }
    }
}