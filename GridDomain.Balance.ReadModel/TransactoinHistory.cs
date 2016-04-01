using System;
using NMoneys;

namespace GridDomain.Balance.ReadModel
{
    public class TransactoinHistory
    {
        public Guid Id { get; set; }
        public Guid BusinessId { get; set; }
        public Guid BalanceId { get; set; }
       
        public Money AmountBeforeTransaction { get; set; }
        public Money AmountAfterTransaction { get; set; }

        public Money TransactionAmount { get; set; }

        public Guid TransactionSource { get; set; }
        public string TransactionType { get; set; }
        public string TransactionDescription { get; set; }
    }
}