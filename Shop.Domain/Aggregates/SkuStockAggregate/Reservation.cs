using System;

namespace Shop.Domain.Aggregates.SkuStockAggregate
{
    public class Reservation
    {
        public Reservation(Guid id, int quantity, DateTime expirationDate)
        {
            Id = id;
            Quantity = quantity;
            ExpirationDate = expirationDate;
        }

        public Guid Id { get; }
        public int Quantity { get; }
        public DateTime ExpirationDate { get; }
    }
}