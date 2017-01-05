using System;
using GridDomain.CQRS;

namespace Shop.Domain.Aggregates.UserAggregate.Commands
{
    public class CompletePendingOrderCommand : Command
    {
        public CompletePendingOrderCommand(Guid userId, Guid orderId)
        {
            UserId = userId;
            OrderId = orderId;
        }

        public Guid UserId { get; }
        public Guid OrderId { get; }
    }
}