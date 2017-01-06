using System;

namespace Shop.Domain.Aggregates.OrderAggregate.Commands
{
    public class CalculateOrderTotalCommand : OrderCommand
    {
        public CalculateOrderTotalCommand(Guid orderId):base(orderId)
        {
            
        }
    }
}