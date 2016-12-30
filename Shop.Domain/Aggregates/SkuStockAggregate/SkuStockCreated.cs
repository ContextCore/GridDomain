using System;
using GridDomain.EventSourcing;

namespace Shop.Domain.Aggregates.SkuStockAggregate
{
    public class SkuStockCreated : DomainEvent
    {
        public Guid SkuId { get; }
        public int Quantity { get; }

        public SkuStockCreated(Guid sourceId, Guid skuId, int quantity):base(sourceId)
        {
            SkuId = skuId;
            Quantity = quantity;
        }
    }
}