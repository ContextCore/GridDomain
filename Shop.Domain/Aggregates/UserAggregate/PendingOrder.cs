using System;

namespace Shop.Domain.Aggregates.UserAggregate
{
    public class PendingOrder
    {
        public PendingOrder(Guid order, Guid skuId, int quantity)
        {
            Order = order;
            SkuId = skuId;
            Quantity = quantity;
        }

        public Guid Order { get; }
        public Guid SkuId { get;}
        public int Quantity { get;  }
    }
}