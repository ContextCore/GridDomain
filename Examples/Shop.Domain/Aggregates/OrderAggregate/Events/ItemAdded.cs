using System;
using GridDomain.EventSourcing;
using NMoneys;

namespace Shop.Domain.Aggregates.OrderAggregate.Events
{
    public class ItemAdded : DomainEvent
    {
        public ItemAdded(Guid sourceId, Guid sku, int quantity, Money totalPrice, int numberInOrder) : base(sourceId)
        {
            Sku = sku;
            Quantity = quantity;
            TotalPrice = totalPrice;
            NumberInOrder = numberInOrder;
        }

        public int NumberInOrder { get; }
        public Guid Sku { get; set; }
        public int Quantity { get; set; }
        public Money TotalPrice { get; set; }
    }
}