using System;

namespace Shop.Domain.Aggregates.OrderAggregate.Commands
{
    class CompleteOrderCommand : OrderCommand
    {
        public CompleteOrderCommand(Guid orderId):base(orderId)
        {

        }
    }
}