using System;
using GridDomain.CQRS;

namespace Shop.Domain.Aggregates.OrderAggregate.Commands
{
    public class OrderCommand : Command
    {
        public Guid OrderId { get; }

        public OrderCommand(Guid orderId)
        {
            OrderId = orderId;
        }
    }
}