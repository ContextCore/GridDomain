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
            await router.RegisterProcess(BuyNow.Descriptor);

            await router.RegisterSyncHandler<AccountCreated, AccountProjectionBuilder>();
            await router.RegisterSyncHandler<AccountReplenish, AccountProjectionBuilder>();
            await router.RegisterSyncHandler<AccountWithdrawal, AccountProjectionBuilder>();

            await router.RegisterSyncHandler<OrderCreated, OrdersProjectionBuilder>();
            await router.RegisterSyncHandler<ItemAdded, OrdersProjectionBuilder>();
            await router.RegisterSyncHandler<OrderCompleted, OrdersProjectionBuilder>();

            await router.RegisterSyncHandler<SkuCreated, SkuProjectionBuilder>();

            await router.RegisterSyncHandler<SkuStockCreated, SkuStockProjectionBuilder>();
            await router.RegisterSyncHandler<StockAdded, SkuStockProjectionBuilder>();
            await router.RegisterSyncHandler<StockReserved, SkuStockProjectionBuilder>();
            await router.RegisterSyncHandler<ReserveExpired, SkuStockProjectionBuilder>();
            await router.RegisterSyncHandler<StockTaken, SkuStockProjectionBuilder>();
            await router.RegisterSyncHandler<StockReserveTaken, SkuStockProjectionBuilder>();
            await router.RegisterSyncHandler<ReserveRenewed, SkuStockProjectionBuilder>();
            await router.RegisterSyncHandler<ReserveCanceled, SkuStockProjectionBuilder>();
        }

        public string Name { get; } = nameof(ShopRouteMap);
    }
}