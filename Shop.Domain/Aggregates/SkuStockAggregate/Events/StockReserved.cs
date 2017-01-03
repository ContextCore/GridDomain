using System;
using GridDomain.EventSourcing;

namespace Shop.Domain.Aggregates.SkuStockAggregate.Events
{
    public class StockReserved:DomainEvent
    {
        public DateTime ExpirationDate { get; }
        public int Quantity { get; }
        public Guid ClientId { get;  }

        public StockReserved(Guid sourceId, Guid clientId, DateTime expirationDate, int quantity):base(sourceId)
        {
            ExpirationDate = expirationDate;
            Quantity = quantity;
            ClientId = clientId;
        }
    }
}