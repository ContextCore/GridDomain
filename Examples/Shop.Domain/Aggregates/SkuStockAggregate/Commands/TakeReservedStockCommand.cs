using System;
using GridDomain.CQRS;

namespace Shop.Domain.Aggregates.SkuStockAggregate.Commands
{
    public class TakeReservedStockCommand : Command
    {
        public TakeReservedStockCommand(Guid stockId, Guid reserveId) : base(stockId)
        {
            ReserveId = reserveId;
        }

        public Guid StockId => AggregateId;
        public Guid ReserveId { get; }
    }
}