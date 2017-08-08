using System;

namespace Shop.ReadModel.Context
{
    public class OrderItem
    {
        public Guid OrderId { get; set; }
        public int NumberInOrder { get; set; }
        public Guid SkuId { get; set; }
        public string SkuName { get; set; }

        public int Quantity { get; set; }
        public decimal TotalPrice { get; set; }
        public string Currency { get; set; }
        public DateTime Created { get; set; }
    }
}