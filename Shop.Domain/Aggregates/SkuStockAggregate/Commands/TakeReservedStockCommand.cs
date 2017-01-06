using System;
using GridDomain.CQRS;

namespace Shop.Domain.Aggregates.SkuStockAggregate.Commands
{
    public class TakeReservedStockCommand:Command
    {
        public TakeReservedStockCommand(Guid stockId, Guid customerId)
        {
            StockId = stockId;
            CustomerId = customerId;
        }

        public Guid StockId { get; }
        public Guid CustomerId { get; }
    }
}