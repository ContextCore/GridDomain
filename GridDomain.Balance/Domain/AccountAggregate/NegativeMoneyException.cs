using System;

namespace GridDomain.Balance.Domain.AccountAggregate
{
    public class NegativeMoneyException : Exception
    {
        public NegativeMoneyException(string msg) : base(msg)
        {
        }
    }
}