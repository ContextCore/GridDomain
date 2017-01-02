using System;
using GridDomain.EventSourcing;

namespace Shop.Domain.Aggregates.SkuStockAggregate.Events
{
    public class StockTaken : DomainEvent
    {
        public int Quantity { get;}

        public StockTaken(Guid sourceId, int quantity):base(sourceId)
        {
            Quantity = quantity;
        }
    }
}