using System;
using GridDomain.CQRS;
using NMoneys;

namespace Shop.Domain.Aggregates.OrderAggregate.Commands
{
    internal class OrderCommand : Command
    {
        public Guid OrderId { get; }

        public OrderCommand(Guid orderId)
        {
            OrderId = orderId;
        }
    }

    class CreateOrderCommand : OrderCommand
    {
        public CreateOrderCommand(Guid orderId, int orderNumber, Guid userId):base(orderId)
        {
            OrderNumber = orderNumber;
            UserId = userId;
        }

        public int OrderNumber { get; }
        public Guid UserId { get; }
    }

    class AddItemToOrderCommand : OrderCommand
    {
        public AddItemToOrderCommand(Guid orderId, Guid skuId, int quantity, Money totalPrice):base(orderId)
        {
            SkuId = skuId;
            Quantity = quantity;
            TotalPrice = totalPrice;
        }

        public Guid SkuId { get; private set; }
        public int Quantity { get; private set; }
        public Money TotalPrice { get; private set; }
    }
    class CompleteOrderCommand : OrderCommand
    {
        public CompleteOrderCommand(Guid orderId):base(orderId)
        {

        }
    }
}
