using System;

namespace Shop.ReadModel.Context
{
    public class SkuStock
    {
        public Guid Id { get; set; }
        public Guid SkuId { get; set; }
        public int TotalQuantity { get; set; }
        public int AvailableQuantity { get; set; }
        public int ReservedQuantity { get; set; }
        public int CustomersReservationsTotal { get; set; }
        public DateTime Created { get; set; }
        public DateTime LastModified { get; set; }
    }
}