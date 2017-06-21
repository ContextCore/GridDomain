using System;

namespace Shop.ReadModel.Context
{
    public class SkuReserve
    {
        public Guid StockId { get; set; }
        public Guid CustomerId { get; set; }
        public DateTime ExpirationDate { get; set; }
        public DateTime Created { get; set; }

        public Guid SkuId { get; set; }
        public int Quantity { get; set; }
    }
}