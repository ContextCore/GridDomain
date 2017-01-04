using System;
using GridDomain.EventSourcing;

namespace Shop.Domain.Aggregates.SkuStockAggregate.Events
{
    public class StockReserveTaken : DomainEvent
    {
        public Guid CustomerId { get; }

        public StockReserveTaken(Guid sourceId, Guid customerId):base(sourceId)
        {
            CustomerId = customerId;
        }
    }
}