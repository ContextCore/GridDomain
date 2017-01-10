using Automatonymous;
using GridDomain.EventSourcing.Sagas.InstanceSagas;
using Shop.Domain.Aggregates.AccountAggregate.Events;
using Shop.Domain.Aggregates.OrderAggregate.Events;
using Shop.Domain.Aggregates.SkuStockAggregate.Events;
using Shop.Domain.Aggregates.UserAggregate.Events;
using Shop.Domain.Sagas;

namespace Shop.Tests.Unit.BuyNowSaga
{
    public sealed class BuyNowPure : Saga<BuyNowData>
    {
   
        public BuyNowPure()
        {
            //Command<CreateOrderCommand>();
            //Command<AddItemToOrderCommand>();
            //Command<ReserveStockCommand>();
            //Command<CalculateOrderTotalCommand>();
            //Command<PayForOrderCommand>();
            //Command<TakeReservedStockCommand>();
            //Command<CompleteOrderCommand>();
            //Command<CompletePendingOrderCommand>();
            InstanceState(x => x.CurrentStateName);
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

                }).TransitionTo(CreatingOrder));

            During(CreatingOrder,
                When(OrderCreated).Then((state, e) =>
                {
                }).TransitionTo(AddingOrderItems));

            During(AddingOrderItems,
                When(ItemAdded).Then((state, e) =>
                {
                }).TransitionTo(Reserving));

            During(Reserving,
                When(StockReserved).Then((state, domainEvent) =>
                {
                    state.ReserveId = domainEvent.ReserveId;
                }),
                When(OrderFinilized).Then((state, domainEvent) =>
                {
                }),
                When(OrderWasReserved).Then(ctx =>
                {
                    int a = 1;
                }).TransitionTo(Paying));


            During(Paying,
                When(OrderPaid, ctx => ctx.Data.ChangeId == ctx.Instance.OrderId).Then((state, e) =>
                {
                }).TransitionTo(TakingStock));

            During(TakingStock,
                When(ReserveTaken).Then((state, e) =>
                {
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
    }
}