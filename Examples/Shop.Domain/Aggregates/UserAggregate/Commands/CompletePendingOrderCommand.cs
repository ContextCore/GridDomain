using System;
using GridDomain.CQRS;

namespace Shop.Domain.Aggregates.UserAggregate.Commands
{
    public class CompletePendingOrderCommand : Command
    {
        public CompletePendingOrderCommand(Guid userId, Guid orderId) : base(orderId)
        {
            UserId = userId;
        }

        public Guid UserId { get; }
        public Guid OrderId => AggregateId;
    }
}