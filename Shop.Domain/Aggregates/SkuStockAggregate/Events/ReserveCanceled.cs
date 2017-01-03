using System;
using GridDomain.EventSourcing;

namespace Shop.Domain.Aggregates.SkuStockAggregate.Events
{
    public class ReserveCanceled:DomainEvent
    {
        public Guid ReservationId { get; }

        public ReserveCanceled(Guid sourceId, Guid reservationId):base(sourceId)
        {
            ReservationId = reservationId;
        }
    }
}