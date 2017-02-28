using System;
using GridDomain.EventSourcing;

namespace Shop.Domain.Aggregates.SkuStockAggregate.Events
{
    public class StockReserveTaken : DomainEvent
    {
        public StockReserveTaken(Guid sourceId, Guid reserveId) : base(sourceId)
        {
            ReserveId = reserveId;
        }

        public Guid ReserveId { get; }
    }
}