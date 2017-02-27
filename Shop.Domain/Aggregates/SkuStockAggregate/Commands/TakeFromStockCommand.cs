using System;
using GridDomain.CQRS;

namespace Shop.Domain.Aggregates.SkuStockAggregate.Commands
{
    public class TakeFromStockCommand:Command
    {
        public TakeFromStockCommand(Guid stockId, int quantity):base(stockId)
        {
            Quantity = quantity;
        }

        public int Quantity { get; }
        public Guid StockId => AggregateId;
    }
}