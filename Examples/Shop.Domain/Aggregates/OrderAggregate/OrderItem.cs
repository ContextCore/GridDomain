using System;
using NMoneys;

namespace Shop.Domain.Aggregates.OrderAggregate
{
    public class OrderItem
    {
        public OrderItem(int number, Guid sku, int quantity, Money totalPrice)
        {
            Number = number;
            Sku = sku;
            Quantity = quantity;
            TotalPrice = totalPrice;
        }

        public int Number { get; }
        public Guid Sku { get; }
        public int Quantity { get; }
        public Money TotalPrice { get; }
    }
}