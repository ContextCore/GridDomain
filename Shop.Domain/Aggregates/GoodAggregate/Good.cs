using System;
using GridDomain.EventSourcing;

namespace Shop.Domain.Aggregates.GoodAggregate
{
    class Good : Aggregate
    {
        public Good(Guid id) : base(id)
        {
        }
    }
}