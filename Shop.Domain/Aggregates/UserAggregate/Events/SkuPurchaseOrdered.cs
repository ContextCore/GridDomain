using System;
using GridDomain.EventSourcing;

namespace Shop.Domain.Aggregates.UserAggregate.Events
{
    public class SkuPurchaseOrdered : DomainEvent
    {
        public SkuPurchaseOrdered(Guid sourceId, Guid skuId, int quantity, Guid orderId, Guid stockId, Guid accountId)
            : base(sourceId)
        {
            SkuId = skuId;
            Quantity = quantity;
            OrderId = orderId;
            StockId = stockId;
            AccountId = accountId;
        }

        public Guid SkuId { get; }
        public int Quantity { get; }
        public Guid OrderId { get; }
        public Guid StockId { get; }
        public Guid AccountId { get; }
    }
}