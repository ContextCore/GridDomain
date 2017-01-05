using System;
using GridDomain.EventSourcing;

namespace Shop.Domain.Aggregates.UserAggregate
{
    public class SkuPurchaseOrdered : DomainEvent
    {
        public Guid SkuId { get;}
        public int Quantity { get; }
        public Guid OrderId { get; }
        public Guid StockId { get; }

        public SkuPurchaseOrdered(Guid sourceId, Guid skuId, int quantity, Guid orderId, Guid stockId):base(sourceId)
        {
            SkuId = skuId;
            Quantity = quantity;
            OrderId = orderId;
            StockId = stockId;
        }
    }
}