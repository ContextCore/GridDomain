using System;
using GridDomain.EventSourcing;

namespace Shop.Domain.Aggregates.UserAggregate.Events
{
    public class PendingOrderCanceled : DomainEvent
    {
        public Guid OrderId { get;}

        public PendingOrderCanceled(Guid sourceId, Guid orderId) : base(sourceId)
        {
            OrderId = orderId;
        }
    }
}