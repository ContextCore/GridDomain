using System;
using CommonDomain.Persistence;
using GridDomain.Balance;
using GridDomain.Balance.Commands;
using GridDomain.Balance.Domain;
using GridDomain.Balance.ReadModel;
using GridDomain.CQRS.Messaging;
using GridDomain.CQRS.Messaging.MessageRouting;

namespace GridDomain.Node.MessageRouteConfigs
{
    public class BalanceCommandsRouting : IMessageRouteConfiguration
    {
        private readonly IRepository _rep;
        private readonly Func<BusinessBalanceContext> _contextCreator;

        public BalanceCommandsRouting(IRepository rep, Func<BusinessBalanceContext> contextCreator)
        {
            _contextCreator = contextCreator;
            this._rep = rep;
        }

        public void Register(IMessagesRouter router)
        {
            router.Route<ReplenishBalanceCommand>()
                     .To<BalanceCommandsHandler>()
                  .Register();

            router.Route<WithdrawalBalanceCommand>()
                     .To<BalanceCommandsHandler>()
                  .Register();

            router.Route<CreateBalanceCommand>()
                  .To<BalanceCommandsHandler>()
               .Register();

            router.Route<BalanceReplenishEvent>()
                    .To<BusinessCurrentBalanceProjectionBuilder>()
                  .Register();

            router.Route<BalanceCreatedEvent>()
                     .To<BusinessCurrentBalanceProjectionBuilder>()
                   .Register();

            router.Route<BalanceWithdrawalEvent>()
                   .To<BusinessCurrentBalanceProjectionBuilder>()
                .Register();
        }
    }

}