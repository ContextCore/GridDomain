using System;
using GridDomain.CQRS;

namespace Shop.Domain.Aggregates.OrderAggregate.Commands
{
    public class OrderCommand : Command
    {
        public Guid OrderId => AggregateId;

        public OrderCommand(Guid orderId):base(orderId)
        {
        }
    }
}