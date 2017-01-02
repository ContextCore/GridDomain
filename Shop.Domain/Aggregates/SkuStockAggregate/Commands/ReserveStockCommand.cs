using System;
using GridDomain.CQRS;

namespace Shop.Domain.Aggregates.SkuStockAggregate.Commands
{
    public class ReserveStockCommand:Command
    {
        public ReserveStockCommand(Guid stockId, Guid customerId, int quantity, Guid reservationId)
        {
            StockId = stockId;
            CustomerId = customerId;
            Quantity = quantity;
            ReservationId = reservationId;
        }

        public int Quantity { get; }
        public Guid ReservationId { get; }
        public Guid CustomerId { get;  }
        public Guid StockId { get;  }
    }
}