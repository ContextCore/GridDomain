using System;
using NMoneys;

namespace Shop.Domain.Aggregates.OrderAggregate.Commands
{
    public class AddItemToOrderCommand : OrderCommand
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
}