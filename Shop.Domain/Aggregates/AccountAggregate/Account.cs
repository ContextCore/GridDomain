using System;
using GridDomain.EventSourcing;

namespace Shop.Domain.Aggregates.AccountAggregate
{
    class Account : Aggregate
    {
        public Account(Guid id) : base(id)
        {
        }
    }
}