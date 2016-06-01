using System;
using System.ComponentModel.DataAnnotations;
using NMoneys;

namespace GridDomain.Balance.ReadModel
{
    public class TransactionHistory
    {
        [Key]
        public Guid Id { get; set; }
        public Guid BalanceId { get; set; }
        public DateTime Time { get; set; }

        public Money TransactionAmount { get; set; }

        public string EventType { get; set; }
        public string Event { get; set; }
    }
}