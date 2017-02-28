using System;
using GridDomain.CQRS;

namespace Shop.Domain.Aggregates.UserAggregate.Commands
{
    public class CancelPendingOrderCommand : Command
    {
        public CancelPendingOrderCommand(Guid userId, Guid orderId) : base(userId)
        {
            OrderId = orderId;
        }

        public Guid UserId => AggregateId;
        public Guid OrderId { get; }
    }
}