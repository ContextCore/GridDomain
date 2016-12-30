using System;
using GridDomain.EventSourcing;

namespace Shop.Domain.Aggregates.SkuStockAggregate
{
    public class SkuStock : Aggregate
    {
        public Guid SkuId { get; private set; }
        public int Quantity { get; private set; }

        private SkuStock(Guid id) : base(id)
        {
            Apply<SkuStockCreated>(e =>
            {
                SkuId = e.SkuId;
                Quantity = e.Quantity;
            });
        }

        public SkuStock(Guid sourceId, Guid skuId, int amount):this(sourceId)
        {
            RaiseEvent(new SkuStockCreated(sourceId,skuId,amount));
        }

    }
}