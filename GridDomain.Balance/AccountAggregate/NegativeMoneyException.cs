using System;

namespace BusinessNews.Domain.AccountAggregate
{
    public class NegativeMoneyException : Exception
    {
        public NegativeMoneyException(string msg) : base(msg)
        {
        }
    }
}