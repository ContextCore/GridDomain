using System;
using GridDomain.CQRS;

namespace Shop.Domain.Aggregates.SkuStockAggregate.Commands
{
    public class TakeReservedStockCommand:Command
    {
        public TakeReservedStockCommand(Guid stockId, int reserveId)
        {
            StockId = stockId;
            ReserveId = reserveId;
        }

        public Guid StockId { get; }
        public int ReserveId { get; }
    }
}