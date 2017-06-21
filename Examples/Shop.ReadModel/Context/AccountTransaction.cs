using System;

namespace Shop.ReadModel.Context
{
    public class AccountTransaction
    {
        public Guid AccountId { get; set; }
        public int TransactionNumber { get; set; }
        public Guid TransactionId { get; set; }
        public AccountOperations Operation { get; set; }
        public decimal InitialAmount { get; set; }
        public decimal NewAmount { get; set; }
        public decimal ChangeAmount { get; set; }
        public string Currency { get; set; }
        public DateTime Created { get; set; }
    }
}