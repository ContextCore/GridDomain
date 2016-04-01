using CommonDomain.Persistence;
using GridDomain.CQRS.Messaging;
using GridDomain.CQRS.Messaging.MessageRouting;

namespace GridDomain.Node.MessageRouteConfigs
{
    public class SkuUnitSalesCommands_RouteConfiguration : IMessageRouteConfiguration
    {
        private readonly IRepository _rep;
        private readonly IPublisher _pub;

        public SkuUnitSalesCommands_RouteConfiguration(IRepository rep, IPublisher pub)
        {
            _pub = pub;
            this._rep = rep;
        }

        public void Register(IMessagesRouter router)
        {
            //router.Route<CalculateSkuSaleCommand>()
            //    .To<SalesCommandsHandler>()
            //    .WithFactory(cmd => new SalesCommandsHandler(_rep, _pub))
            //  .Register();

            //router.Route<BeginSkuSalesHistoryCommand>()
            //    .To<SalesCommandsHandler>()
            //    .WithFactory(cmd => new SalesCommandsHandler(_rep, _pub))
            //  .Register();

            //router.Route<AddShopToSellersCommand>()
            //    .To<SellersCommandHandlers>()
            //    .WithFactory(cmd => new SellersCommandHandlers(_rep, _pub))
            //  .Register();

            //router.Route<CreateSkuSalesNetCommand>()
            //    .To<SkuUnitCommandsHandler>()
            //    .WithFactory(cmd => new SkuUnitCommandsHandler(_rep, _pub))
            //  .Register();

            //router.Route<CreateSkuCommand>()
            //    .To<SkuUnitCommandsHandler>()
            //    .WithFactory(cmd => new SkuUnitCommandsHandler(_rep, _pub))
            //  .Register();

            //router.Route<CreateDepartmentCommand>()
            //    .To<SkuUnitCommandsHandler>()
            //    .WithFactory(cmd => new SkuUnitCommandsHandler(_rep, _pub))
            //  .Register();
        }
    }
}