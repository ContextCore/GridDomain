using System;
using CommonDomain.Core;
using NMoneys;

namespace BusinessNews.Domain
{
    public class TransactoinHistory : AggregateBase
    {
        private TransactoinHistory(Guid id)
        {
            Id = id;
        }

        public TransactoinHistory(Guid id, Guid balanceId, Guid businessId, Money amount, Guid sourceId)
        {
            RaiseEvent(new TransactoinCreatedEvent(sourceId)
            {
                BalanceId = balanceId,
                BusinessId = businessId,
                TransactionAmount = amount,
                TransactionId = id,
                TransactionSource = sourceId
            });
        }

        public Guid BusinessId { get; private set; }
        public Guid BalanceId { get; private set; }

        public Money TransactionAmount { get; private set; }

        public Guid TransactionSource { get; private set; }

        private void Apply(TransactoinCreatedEvent e)
        {
            BusinessId = e.BusinessId;
            BalanceId = e.BalanceId;
            TransactionAmount = e.TransactionAmount;
            TransactionSource = e.TransactionSource;
            Id = e.TransactionId;
        }
    }
}