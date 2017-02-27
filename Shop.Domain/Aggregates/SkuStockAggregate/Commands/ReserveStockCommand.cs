using System;
using GridDomain.CQRS;

namespace Shop.Domain.Aggregates.SkuStockAggregate.Commands
{
    public class ReserveStockCommand:Command
    {
        public ReserveStockCommand(Guid stockId, Guid customerId, int quantity, DateTime? reservationStartTime = null)
            :base(stockId)
        {
            CustomerId = customerId;
            Quantity = quantity;
            ReservationStartTime = reservationStartTime;
        }

        public int Quantity { get; }
        public DateTime? ReservationStartTime { get;}
        public Guid CustomerId { get;  }
        public Guid StockId => AggregateId;
    }
}