using System;
using NMoneys;

namespace BusinessNews.Domain.BillAggregate
{
    public struct Charge
    {
        public Charge(Guid id, Money amount)
        {
            Id = id;
            Amount = amount;
        }

        public Guid Id { get; }
        public Money Amount { get; }
    }
}