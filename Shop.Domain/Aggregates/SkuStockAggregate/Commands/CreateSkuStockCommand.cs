using System;
using GridDomain.CQRS;

namespace Shop.Domain.Aggregates.SkuStockAggregate.Commands
{
    public class CreateSkuStockCommand : Command
    {
        public CreateSkuStockCommand(Guid stockId, Guid skuId, int quantity, string batchArticle, TimeSpan reserveTime):
            base(stockId)
        {
            SkuId = skuId;
            Quantity = quantity;
            BatchArticle = batchArticle;
            ReserveTime = reserveTime;
        }

        public Guid StockId => AggregateId;
        public Guid SkuId { get; }
        public int Quantity { get; }
        public string BatchArticle { get; }
        public TimeSpan ReserveTime { get; }
    }
}