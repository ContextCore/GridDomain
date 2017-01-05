using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Automatonymous;
using GridDomain.CQRS;
using GridDomain.EventSourcing.Sagas;
using GridDomain.EventSourcing.Sagas.InstanceSagas;
using NMoneys;
using Shop.Domain.Aggregates.AccountAggregate.Commands;
using Shop.Domain.Aggregates.OrderAggregate;
using Shop.Domain.Aggregates.OrderAggregate.Commands;
using Shop.Domain.Aggregates.OrderAggregate.Events;
using Shop.Domain.Aggregates.SkuStockAggregate.Commands;
using Shop.Domain.Aggregates.SkuStockAggregate.Events;
using Shop.Domain.Aggregates.UserAggregate;
using Shop.Domain.Aggregates.UserAggregate.Commands;

namespace Shop.Domain.Sagas
{

    class BuyStockSaga : Saga<BuyStockSagaData>
    {
        public static readonly ISagaDescriptor Descriptor
            = SagaExtensions.CreateDescriptor<BuyStockSaga,
                                              BuyStockSagaData,
                                              SkuPurchaseOrdered>(new BuyStockSaga(null));

        public BuyStockSaga(IPriceCalculator calculator)
        {
            Command<ReserveStockCommand>();
            Command<PayForOrderCommand>();
            Command<TakeReservedStockCommand>();

            Event(() => PurchaseOrdered);
            Event(() => ItemAdded);
            Event(() => OrderCreated);

            During(ReceivingPurchaseOrder,
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
                }).TransitionTo(CreatingOrder));

            During(CreatingOrder,
                When(OrderCreated).Then(ctx =>
                {
                    var state = ctx.Instance;
                    var totalPrice = calculator.CalculatePrice(state.SkuId, state.Quantity);
                    Dispatch(new AddItemToOrderCommand(state.OrderId,
                                                       state.SkuId,
                                                       state.Quantity,
                                                       totalPrice));
                }).TransitionTo(AddingOrderItems));

            During(AddingOrderItems,
                When(ItemAdded).Then(ctx =>
                {
                    var state = ctx.Instance;
                    Dispatch(new ReserveStockCommand(state.StockId,state.UserId,state.Quantity));
                }).TransitionTo(Reserving));

          //  During(Reserving,
          //         When(StockReserved).Then(ctx =>
          //         {
          //             Dispatch(new PayForOrderCommand(state.StockId, state.UserId, state.Quantity));
          //         }).TransitionTo(Reserving));
          //

        }

      // public Event<GotTiredEvent> GotTired { get; private set; }
      // public Event<CoffeMadeEvent> CoffeReady { get; private set; }
      // public Event<SleptWellEvent> SleptWell { get; private set; }
      // public Event<Fault<GoSleepCommand>> SleptBad { get; private set; }
      // public Event<CoffeMakeFailedEvent> CoffeNotAvailable { get; private set; }

        public Event<SkuPurchaseOrdered> PurchaseOrdered { get; private set; }
        public Event<OrderCreated> OrderCreated { get; private set; }
        public Event<ItemAdded> ItemAdded { get; private set; }
        public Event<StockReserved> StockReserved { get; private set; }

        public State ReceivingPurchaseOrder{ get; private set; }
        public State CreatingOrder { get; private set; }
        public State AddingOrderItems { get; private set; }
        public State CreatingBill { get; private set; }
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
        public Guid ReserveId { get; set; }
        public Money TotalOrderCost { get; set; }
    }
}
