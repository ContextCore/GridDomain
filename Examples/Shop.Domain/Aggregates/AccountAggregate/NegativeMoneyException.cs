using System;

namespace Shop.Domain.Aggregates.AccountAggregate
{
    public class NegativeMoneyException : Exception
    {
        public NegativeMoneyException(string msg) : base(msg) {}
    }
}