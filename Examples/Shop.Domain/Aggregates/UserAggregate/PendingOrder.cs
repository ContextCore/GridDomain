using System;

namespace Shop.Domain.Aggregates.UserAggregate
{
    public class PendingOrder
    {
        public PendingOrder(Guid order, Guid skuId, int quantity, Guid stockId)
        {
            Order = order;
            SkuId = skuId;
            Quantity = quantity;
            StockId = stockId;
        }

        public Guid Order { get; }
        public Guid SkuId { get; }
        public int Quantity { get; }
        public Guid StockId { get; }
    }
}