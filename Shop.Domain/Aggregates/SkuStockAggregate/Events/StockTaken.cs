using System;
using GridDomain.EventSourcing;

namespace Shop.Domain.Aggregates.SkuStockAggregate.Events
{
    public class StockTaken : DomainEvent
    {
        public StockTaken(Guid sourceId, int quantity) : base(sourceId)
        {
            Quantity = quantity;
        }

        public int Quantity { get; }
    }
}