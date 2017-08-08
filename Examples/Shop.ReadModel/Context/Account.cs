using System;

namespace Shop.ReadModel.Context
{
    public class Account
    {
        public Guid Id { get; set; }
        public int Number { get; set; }
        public Guid UserId { get; set; }
        public string Login { get; set; }
        public DateTime Created { get; set; }
        public DateTime LastModified { get; set; }
        public string Currency { get; set; }
        public decimal Amount { get; set; }
    }
}