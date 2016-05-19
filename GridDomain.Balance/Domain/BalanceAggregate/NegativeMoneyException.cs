using System;

namespace GridDomain.Balance.Domain.BalanceAggregate
{
    public class NegativeMoneyException : Exception
    {
        public NegativeMoneyException(string msg) : base(msg)
        {
        }
    }
}