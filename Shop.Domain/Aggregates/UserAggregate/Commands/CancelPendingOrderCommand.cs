using System;
using GridDomain.CQRS;

namespace Shop.Domain.Aggregates.UserAggregate.Commands
{
    public class CancelPendingOrderCommand : Command
    {
        public CancelPendingOrderCommand(Guid userId, Guid orderId)
        {
            UserId = userId;
            OrderId = orderId;
        }

        public Guid UserId { get; }
        public Guid OrderId { get; }
    }
}