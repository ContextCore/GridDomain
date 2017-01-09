using System;
using GridDomain.CQRS;

namespace Shop.Domain.Aggregates.SkuStockAggregate.Commands
{
    public class TakeReservedStockCommand:Command
    {
        public TakeReservedStockCommand(Guid stockId, Guid reserveId)
        {
            StockId = stockId;
            ReserveId = reserveId;
        }

        public Guid StockId { get; }
        public Guid ReserveId { get; }
    }
}