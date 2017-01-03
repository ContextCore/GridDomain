using System;
using GridDomain.EventSourcing;

namespace Shop.Domain.Aggregates.SkuStockAggregate.Events
{
    public class ReserveRenewed : DomainEvent
    {
        public Guid CustomerId { get; }

        public ReserveRenewed(Guid sourceId, Guid customerId):base(sourceId)
        {
            CustomerId = customerId;
        }
    }
}