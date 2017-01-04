using System;

namespace Shop.ReadModel.Context
{
    public class SkuStockHistory
    {
        public int Number { get; set; }
        public Guid StockId { get; set; }
        public int Quanity { get; set; }
        public SkuStockOperation Operation { get; set; }
        public int OldAvailableQuantity { get; set; }
        public int NewAvailableQuantity { get; set; }
        public int OldReservedQuantity { get; set; }
        public int NewReservedQuantity { get; set; }
        public int OldTotalQuantity { get; set; }
        public int NewTotalQuantity { get; set; }
    }
}