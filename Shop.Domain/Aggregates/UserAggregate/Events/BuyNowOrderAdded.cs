using System;
using GridDomain.EventSourcing;

namespace Shop.Domain.Aggregates.UserAggregate
{
    public class BuyNowOrderAdded : DomainEvent
    {
        public Guid SkuId { get;}
        public int Quantity { get; }
        public Guid OrderId { get; set; }

        public BuyNowOrderAdded(Guid sourceId, Guid skuId, int quantity, Guid orderId):base(sourceId)
        {
            SkuId = skuId;
            Quantity = quantity;
            OrderId = orderId;
        }
    }
}