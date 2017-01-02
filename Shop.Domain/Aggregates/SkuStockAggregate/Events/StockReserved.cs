using System;
using GridDomain.EventSourcing;

namespace Shop.Domain.Aggregates.SkuStockAggregate.Events
{
    public class StockReserved:DomainEvent
    {
        public Guid ReservationId { get; }
        public DateTime ExpirationDate { get; }
        public int Quantity { get; }

        public StockReserved(Guid sourceId, Guid reservationId, DateTime expirationDate, int quantity):base(sourceId)
        {
            ReservationId = reservationId;
            ExpirationDate = expirationDate;
            Quantity = quantity;
        }
    }
}