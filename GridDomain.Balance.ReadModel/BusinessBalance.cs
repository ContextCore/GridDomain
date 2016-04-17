using System;
using System.ComponentModel.DataAnnotations;

namespace GridDomain.Balance.ReadModel
{
    public class BusinessBalance
    {
        [Key]
        public Guid BalanceId { get; set; }

        public Guid BusinessId { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; }

        [Timestamp]
        [ConcurrencyCheck]
        public byte[] RowVersion { get; set; }
    }
}