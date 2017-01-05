using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Automatonymous;
using GridDomain.CQRS;
using GridDomain.EventSourcing.Sagas;
using GridDomain.EventSourcing.Sagas.InstanceSagas;
using Shop.Domain.Aggregates.AccountAggregate.Commands;
using Shop.Domain.Aggregates.OrderAggregate.Commands;
using Shop.Domain.Aggregates.SkuStockAggregate.Commands;
using Shop.Domain.Aggregates.UserAggregate;

namespace Shop.Domain.Sagas
{
    class BuyStockSaga : Saga<BuyStockSagaData>
    {
        public static readonly ISagaDescriptor Descriptor
            = SagaExtensions.CreateDescriptor<BuyStockSaga,
                                              BuyStockSagaData,
                                              SkuPurchaseOrdered>();

        public BuyStockSaga()
        {
            Command<ReserveStockCommand>();
            Command<PayForOrderCommand>();
            Command<TakeReservedStockCommand>();

            Event(() => PurchaseOrdered);

            During(CreatingOrder,
                When(PurchaseOrdered).Then(ctx =>
                {
                    var state = ctx.Instance;
                    var domainEvent = ctx.Data;
                    state.AccountId = domainEvent.AccountId;
                    state.OrderId = domainEvent.OrderId;
                    state.Quantity = domainEvent.Quantity;
                    state.SkuId = domainEvent.SkuId;
                    state.UserId = domainEvent.SourceId;
                    state.StockId = domainEvent.StockId;

                    Dispatch(new CreateOrderCommand(state.OrderId,state.UserId));
                    //  Dispatch(new ReserveStockCommand(state.StockId,state.UserId,state.Quantity));
                }));

        }

      // public Event<GotTiredEvent> GotTired { get; private set; }
      // public Event<CoffeMadeEvent> CoffeReady { get; private set; }
      // public Event<SleptWellEvent> SleptWell { get; private set; }
      // public Event<Fault<GoSleepCommand>> SleptBad { get; private set; }
      // public Event<CoffeMakeFailedEvent> CoffeNotAvailable { get; private set; }

        public Event<SkuPurchaseOrdered> PurchaseOrdered { get; private set; }
        public State CreatingOrder { get; private set; }
        public State Reserving { get; private set; }
        public State Paying { get; private set; }
        public State TakingStock { get; private set; }
    }

    internal class BuyStockSagaData : ISagaState
    {
        public string CurrentStateName { get; set; }
        public Guid UserId { get; set; }
        public Guid SkuId { get; set; }
        public Guid AccountId { get; set; }
        public Guid OrderId { get; set; }
        public Guid StockId { get; set; }
        public int Quantity { get; set; }
    }
}
