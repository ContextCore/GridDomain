using System;
using CommonDomain.Persistence;
using GridDomain.Balance.ReadModel;
using GridDomain.CQRS.Messaging;
using GridDomain.CQRS.Messaging.MessageRouting;

namespace GridDomain.Node.MessageRouteConfigs
{
    public static class MessageRouting
    {
        public static void Init(IRepository repository,
            IPublisher messageTransport,
            Func<BusinessBalanceContext> contextCreator,
            IMessagesRouter messagesRouter)
        {
            var routeConfigs = new IMessageRouteConfiguration[]
            {
                //  new SkuUnitSalesCommands_RouteConfiguration(repository,messageTransport),
                //  new ProcessRecieptSaga_RouteConfiguration(repository,messageTransport),
                // new ProcessSkuSaleSaga_RouteConfiguration(repository,messageTransport),
                new BalanceCommandsRouting(repository, contextCreator)
            };

            foreach (var routeConfig in routeConfigs)
            {
                routeConfig.Register(messagesRouter);
            }
        }
    }
}