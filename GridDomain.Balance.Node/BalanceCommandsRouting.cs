using System;
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
        public void Register(IMessagesRouter router)
        {
            router.Route<ReplenishBalanceCommand>()
                     .To<BalanceCommandsHandler>()
                     .WithCorrelation(nameof(ReplenishBalanceCommand.BalanceId))
                  .Register();

            router.Route<WithdrawalBalanceCommand>()
                     .To<BalanceCommandsHandler>()
                     .WithCorrelation(nameof(WithdrawalBalanceCommand.BalanceId))
                  .Register();

            router.Route<CreateBalanceCommand>()
                  .To<BalanceCommandsHandler>()
                   .WithCorrelation(nameof(CreateBalanceCommand.BalanceId))
               .Register();

            router.Route<BalanceReplenishEvent>()
                    .To<BusinessCurrentBalanceProjectionBuilder>()
                    .WithCorrelation(nameof(BalanceReplenishEvent.BalanceId))
                  .Register();

            router.Route<BalanceCreatedEvent>()
                     .To<BusinessCurrentBalanceProjectionBuilder>()
                      .WithCorrelation(nameof(BalanceCreatedEvent.BalanceId))
                   .Register();

            router.Route<BalanceWithdrawalEvent>()
                   .To<BusinessCurrentBalanceProjectionBuilder>()
                    .WithCorrelation(nameof(BalanceWithdrawalEvent.BalanceId))
                .Register();
        }
    }

}