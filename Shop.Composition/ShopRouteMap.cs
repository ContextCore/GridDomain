using System.Threading.Tasks;
using GridDomain.CQRS.Messaging;
using GridDomain.CQRS.Messaging.MessageRouting;
using Shop.Domain.Aggregates.AccountAggregate;
using Shop.Domain.Aggregates.AccountAggregate.Events;
using Shop.Domain.Aggregates.OrderAggregate;
using Shop.Domain.Aggregates.OrderAggregate.Events;
using Shop.Domain.Aggregates.SkuAggregate;
using Shop.Domain.Aggregates.SkuAggregate.Events;
using Shop.Domain.Aggregates.SkuStockAggregate;
using Shop.Domain.Aggregates.SkuStockAggregate.Events;
using Shop.Domain.Aggregates.UserAggregate;
using Shop.Domain.Sagas;
using Shop.ReadModel;

namespace Shop.Composition
{
    public class ShopRouteMap : IMessageRouteMap
    {
        public async Task Register(IMessagesRouter router)
        {
            await router.RegisterAggregate<Account, AccountCommandsHandler>();
            await router.RegisterAggregate(new OrderCommandsHandler(null));
            await router.RegisterAggregate(new SkuCommandsHandler(null));
            await router.RegisterAggregate<SkuStock, SkuStockCommandsHandler>();
            await router.RegisterAggregate(new UserCommandsHandler(null));
            await router.RegisterSaga(BuyNow.Descriptor);

            await router.RegisterHandler<AccountCreated, AccountProjectionBuilder>();
            await router.RegisterHandler<AccountReplenish, AccountProjectionBuilder>();
            await router.RegisterHandler<AccountWithdrawal, AccountProjectionBuilder>();

            await router.RegisterHandler<OrderCreated, OrdersProjectionBuilder>();
            await router.RegisterHandler<ItemAdded, OrdersProjectionBuilder>();
            await router.RegisterHandler<OrderCompleted, OrdersProjectionBuilder>();

            await router.RegisterHandler<SkuCreated, SkuProjectionBuilder>();

            await router.RegisterHandler<SkuStockCreated, SkuStockProjectionBuilder>();
            await router.RegisterHandler<StockAdded, SkuStockProjectionBuilder>();
            await router.RegisterHandler<StockReserved, SkuStockProjectionBuilder>();
            await router.RegisterHandler<ReserveExpired, SkuStockProjectionBuilder>();
            await router.RegisterHandler<StockTaken, SkuStockProjectionBuilder>();
            await router.RegisterHandler<StockReserveTaken, SkuStockProjectionBuilder>();
            await router.RegisterHandler<ReserveRenewed, SkuStockProjectionBuilder>();
            await router.RegisterHandler<ReserveCanceled, SkuStockProjectionBuilder>();
        }
    }
}