using System;
using GridDomain.EventSourcing;
using NMoneys;

namespace Shop.Domain.Aggregates.OrderAggregate
{
    public class ItemAdded : DomainEvent
    {
        public Guid Sku { get; set; }
        public int Quantity { get; set; }
        public Money TotalPrice { get; set; }

        public ItemAdded(Guid sourceId, Guid sku, int quantity, Money totalPrice):base(sourceId)
        {
            Sku = sku;
            Quantity = quantity;
            TotalPrice = totalPrice;
        }
    }
}