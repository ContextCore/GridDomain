using System;
using GridDomain.EventSourcing;

namespace Shop.Domain.Aggregates.SkuStockAggregate.Events
{
    public class ReserveCanceled:DomainEvent
    {
        public Guid CustomerId { get; }

        public ReserveCanceled(Guid sourceId, Guid customerId):base(sourceId)
        {
            CustomerId = customerId;
        }
    }
}