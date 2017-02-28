using System;
using GridDomain.EventSourcing;

namespace Shop.Domain.Aggregates.SkuStockAggregate.Events
{
    public class ReserveCanceled : DomainEvent
    {
        public ReserveCanceled(Guid sourceId, Guid reserveId) : base(sourceId)
        {
            ReserveId = reserveId;
        }

        public Guid ReserveId { get; }
    }
}