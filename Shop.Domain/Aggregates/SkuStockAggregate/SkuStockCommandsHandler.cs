using GridDomain.CQRS.Messaging.MessageRouting;

namespace Shop.Domain.Aggregates.SkuStockAggregate
{
    public class SkuStockCommandsHandler : AggregateCommandsHandler<SkuStock>
    {
        public SkuStockCommandsHandler()
        {
            Map<CreateSkuStockCommand>(c => c.StockId,
                c => new SkuStock(c.StockId, c.SkuId, c.Quantity));

            Map<AddToStockCommand>(c => c.StockId,
                (c,a) => a.AddToToStock(c.Quantity,c.SkuId,c.BatchArticle));
        }
    }
}