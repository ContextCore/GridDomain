using System;
using GridDomain.EventSourcing;

namespace Shop.Domain.Aggregates.SkuStockAggregate.Events
{
    public class StockReserved:DomainEvent
    {
        public DateTime ExpirationDate { get; }
        public int Quantity { get; }
        public Guid ReserveId { get;  }

        public StockReserved(Guid sourceId, Guid reserveId, DateTime expirationDate, int quantity):base(sourceId)
        {
            ExpirationDate = expirationDate;
            Quantity = quantity;
            ReserveId = reserveId;
        }
    }
}