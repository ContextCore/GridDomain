using System;
using GridDomain.EventSourcing;

namespace Shop.Domain.Aggregates.UserAggregate.Events
{
    public class PendingOrderCompleted : DomainEvent
    {
        public Guid OrderId { get; }

        public PendingOrderCompleted(Guid sourceId, Guid orderId):base(sourceId)
        {
            OrderId = orderId;
        }
    }
}