using System;

namespace Shop.Domain.Aggregates.OrderAggregate.Commands
{
    public class CreateOrderCommand : OrderCommand
    {
        public CreateOrderCommand(Guid orderId, Guid userId):base(orderId)
        {
            UserId = userId;
        }

        public Guid UserId { get; }
    }
}
