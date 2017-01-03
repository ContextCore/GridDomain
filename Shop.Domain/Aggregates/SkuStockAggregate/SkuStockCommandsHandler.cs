using GridDomain.CQRS.Messaging.MessageRouting;
using Shop.Domain.Aggregates.SkuStockAggregate.Commands;

namespace Shop.Domain.Aggregates.SkuStockAggregate
{
    public class SkuStockCommandsHandler : AggregateCommandsHandler<SkuStock>
    {
        public SkuStockCommandsHandler()
        {
            Map<CreateSkuStockCommand>(c => c.StockId,
                c => new SkuStock(c.StockId, c.SkuId, c.Quantity, c.ReserveTime));

            Map<AddToStockCommand>(c => c.StockId,
                (c,a) => a.AddToToStock(c.Quantity,c.SkuId,c.BatchArticle));

            Map<TakeFromStockCommand>(c => c.StockId,
                (c,a) => a.Take(c.Quantity));

            Map<ReserveStockCommand>(c => c.StockId,
                (c, a) => a.Reserve(c.CustomerId, c.Quantity, c.ReservationId, c.ReservationStartTime));

            Map<TakeReservedStockCommand>(c => c.StockId,
                (c, a) => a.Take(c.ReserveId));
        }
    }
}