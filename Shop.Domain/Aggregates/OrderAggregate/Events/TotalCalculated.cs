using System;
using GridDomain.EventSourcing;
using NMoneys;

namespace Shop.Domain.Aggregates.OrderAggregate
{
    public class TotalCalculated : DomainEvent
    {
        public Money TotalPrice { get; }

        public TotalCalculated(Guid sourceId, Money totalPrice) : base(sourceId)
        {
            TotalPrice = totalPrice;
        }
    }
}