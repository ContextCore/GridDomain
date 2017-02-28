using Automatonymous;
using GridDomain.EventSourcing.Sagas;
using GridDomain.EventSourcing.Sagas.InstanceSagas;
using Shop.Domain.Aggregates.AccountAggregate.Commands;
using Shop.Domain.Aggregates.AccountAggregate.Events;
using Shop.Domain.Aggregates.OrderAggregate.Commands;
using Shop.Domain.Aggregates.OrderAggregate.Events;
using Shop.Domain.Aggregates.SkuStockAggregate.Commands;
using Shop.Domain.Aggregates.SkuStockAggregate.Events;
using Shop.Domain.Aggregates.UserAggregate.Commands;
using Shop.Domain.Aggregates.UserAggregate.Events;
using Shop.Domain.DomainServices.PriceCalculator;

namespace Shop.Domain.Sagas
{
    public class BuyNow : Saga<BuyNowData>
    {
        public static readonly ISagaDescriptor Descriptor = CreateDescriptor();

        public BuyNow(IPriceCalculator calculator)
        {
            CompositeEvent(() => OrderWasReserved, x => x.OrderWarReservedStatus, OrderFinilized, StockReserved);

            During(Initial,
                When(PurchaseOrdered).Then((state, domainEvent) =>
                                           {
                                               state.AccountId = domainEvent.AccountId;
                                               state.OrderId = domainEvent.OrderId;
                                               state.Quantity = domainEvent.Quantity;
                                               state.SkuId = domainEvent.SkuId;
                                               state.UserId = domainEvent.SourceId;
                                               state.StockId = domainEvent.StockId;

                                               Dispatch(new CreateOrderCommand(state.OrderId, state.UserId));
                                           }).TransitionTo(CreatingOrder));

            During(CreatingOrder,
                When(OrderCreated).ThenAsync(async (state, e) =>
                                                   {
                                                       var totalPrice =
                                                           await calculator.CalculatePrice(state.SkuId, state.Quantity);
                                                       Dispatch(new AddItemToOrderCommand(state.OrderId,
                                                           state.SkuId,
                                                           state.Quantity,
                                                           totalPrice));
                                                   }).TransitionTo(AddingOrderItems));

            During(AddingOrderItems,
                When(ItemAdded)
                    .Then((state, e) => { Dispatch(new ReserveStockCommand(state.StockId, state.UserId, state.Quantity)); })
                    .TransitionTo(Reserving));

            During(Reserving,
                When(StockReserved).Then((state, domainEvent) =>
                                         {
                                             state.ReserveId = domainEvent.ReserveId;
                                             Dispatch(new CalculateOrderTotalCommand(state.OrderId));
                                         }),
                When(OrderFinilized)
                    .Then(
                        (state, domainEvent) =>
                        {
                            Dispatch(new PayForOrderCommand(state.AccountId, domainEvent.TotalPrice, state.OrderId));
                        }),
                When(OrderWasReserved).TransitionTo(Paying));


            During(Paying,
                When(OrderPaid, ctx => ctx.Data.ChangeId == ctx.Instance.OrderId)
                    .Then((state, e) => { Dispatch(new TakeReservedStockCommand(state.StockId, state.ReserveId)); })
                    .TransitionTo(TakingStock));

            During(TakingStock,
                When(ReserveTaken).Then((state, e) =>
                                        {
                                            Dispatch(new CompleteOrderCommand(state.OrderId));
                                            Dispatch(new CompletePendingOrderCommand(state.UserId, state.OrderId));
                                        }).Finalize());
        }

        public Event<SkuPurchaseOrdered> PurchaseOrdered { get; private set; }
        public Event OrderWasReserved { get; private set; }
        public Event<OrderCreated> OrderCreated { get; private set; }
        public Event<ItemAdded> ItemAdded { get; private set; }
        public Event<StockReserved> StockReserved { get; private set; }
        public Event<OrderTotalCalculated> OrderFinilized { get; private set; }
        public Event<AccountWithdrawal> OrderPaid { get; private set; }
        public Event<StockReserveTaken> ReserveTaken { get; private set; }

        public State CreatingOrder { get; private set; }
        public State AddingOrderItems { get; private set; }
        public State Reserving { get; private set; }
        public State Paying { get; private set; }
        public State TakingStock { get; private set; }

        private static SagaDescriptor CreateDescriptor()
        {
            var descriptor = SagaDescriptor.CreateDescriptor<BuyNow, BuyNowData>();

            descriptor.AddStartMessage<SkuPurchaseOrdered>();

            descriptor.AddCommand<CreateOrderCommand>();
            descriptor.AddCommand<AddItemToOrderCommand>();
            descriptor.AddCommand<ReserveStockCommand>();
            descriptor.AddCommand<CalculateOrderTotalCommand>();
            descriptor.AddCommand<PayForOrderCommand>();
            descriptor.AddCommand<TakeReservedStockCommand>();
            descriptor.AddCommand<CompleteOrderCommand>();
            descriptor.AddCommand<CompletePendingOrderCommand>();

            return descriptor;
        }
    }
}