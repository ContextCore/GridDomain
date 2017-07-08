using GridDomain.CQRS.Messaging;
using GridDomain.CQRS.Messaging.MessageRouting;
using GridDomain.EventSourcing;
using GridDomain.Scheduling.FutureEvents;
using Shop.Domain.Aggregates.SkuStockAggregate.Commands;

namespace Shop.Domain.Aggregates.SkuStockAggregate
{
    public class SkuStockCommandsHandler : AggregateCommandsHandler<SkuStock>
    {
        public SkuStockCommandsHandler()
        {
            Map<CreateSkuStockCommand>(c => new SkuStock(c.StockId, c.SkuId, c.Quantity, c.ReserveTime));

            Map<AddToStockCommand>((c, a) => a.AddToToStock(c.Quantity, c.SkuId, c.BatchArticle));

            Map<TakeFromStockCommand>((c, a) => a.Take(c.Quantity));

            Map<ReserveStockCommand>((c, a) => a.Reserve(c.CustomerId, c.Quantity, c.ReservationStartTime));

            Map<TakeReservedStockCommand>((c, a) => a.Take(c.ReserveId));

            this.MapFutureEvents();
        }
    }
}