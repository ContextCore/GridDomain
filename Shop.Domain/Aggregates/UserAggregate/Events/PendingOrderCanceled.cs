using System;
using GridDomain.EventSourcing;

namespace Shop.Domain.Aggregates.UserAggregate.Events
{
    public class PendingOrderCanceled : DomainEvent
    {
        public PendingOrderCanceled(Guid sourceId, Guid orderId) : base(sourceId)
        {
            OrderId = orderId;
        }

        public Guid OrderId { get; }
    }
}