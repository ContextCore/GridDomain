using System;
using GridDomain.EventSourcing;

namespace Shop.Domain.Aggregates.SkuStockAggregate.Events
{
    public class SkuStockCreated : DomainEvent
    {
        public SkuStockCreated(Guid sourceId, Guid skuId, int quantity, TimeSpan reservationTime) : base(sourceId)
        {
            SkuId = skuId;
            Quantity = quantity;
            ReservationTime = reservationTime;
        }

        public Guid SkuId { get; }
        public int Quantity { get; }
        public TimeSpan ReservationTime { get; }
    }
}