using System;
using GridDomain.EventSourcing;
using NMoneys;

namespace Shop.Domain.Aggregates.OrderAggregate.Events
{
    public class OrderTotalCalculated : DomainEvent
    {
        public Money TotalPrice { get; }

        public OrderTotalCalculated(Guid sourceId, Money totalPrice) : base(sourceId)
        {
            TotalPrice = totalPrice;
        }
    }
}