using System;

namespace Shop.Domain.Aggregates.SkuStockAggregate
{
    public class Reservation
    {
        public Reservation(int quantity, DateTime expirationDate)
        {
            Quantity = quantity;
            ExpirationDate = expirationDate;
        }

        public int Quantity { get; }
        public DateTime ExpirationDate { get; }
    }
}