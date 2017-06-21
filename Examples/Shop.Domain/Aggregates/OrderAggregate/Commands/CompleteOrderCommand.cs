using System;

namespace Shop.Domain.Aggregates.OrderAggregate.Commands
{
    internal class CompleteOrderCommand : OrderCommand
    {
        public CompleteOrderCommand(Guid orderId) : base(orderId) {}
    }
}