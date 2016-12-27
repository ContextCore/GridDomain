using System;

namespace Shop.Domain.Aggregates.OrderAggregate.Commands
{
    public class CreateOrderCommand : OrderCommand
    {
        public CreateOrderCommand(Guid orderId, int orderNumber, Guid userId):base(orderId)
        {
            OrderNumber = orderNumber;
            UserId = userId;
        }

        public int OrderNumber { get; }
        public Guid UserId { get; }
    }
}
