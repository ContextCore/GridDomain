using System;
using GridDomain.CQRS;

namespace Shop.Domain.Aggregates.SkuStockAggregate
{
    public class CreateSkuStockCommand : Command
    {
        public CreateSkuStockCommand(Guid stockId, Guid skuId, int quantity, string batchArticle)
        {
            StockId = stockId;
            SkuId = skuId;
            Quantity = quantity;
            BatchArticle = batchArticle;
        }

        public Guid StockId { get; }
        public Guid SkuId { get; }
        public int Quantity { get; }
        public string BatchArticle { get; }
    }
}