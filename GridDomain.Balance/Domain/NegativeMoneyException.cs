using System;

namespace GridDomain.Balance.Domain
{
    public class NegativeMoneyException : Exception
    {
        public NegativeMoneyException(string msg) : base(msg)
        {
        }
    }
}