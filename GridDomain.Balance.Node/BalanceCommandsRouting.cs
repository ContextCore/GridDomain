using GridDomain.Balance.Domain.BalanceAggregate;
using GridDomain.Balance.Domain.BalanceAggregate.Commands;
using GridDomain.Balance.Domain.BalanceAggregate.Events;
using GridDomain.Balance.ReadModel;
using GridDomain.CQRS.Messaging;
using GridDomain.CQRS.Messaging.MessageRouting;
using GridDomain.Node.AkkaMessaging;

namespace GridDomain.Balance.Node
{
    public class BalanceCommandsRouting : IMessageRouteConfiguration
    {
        public void Register(IMessagesRouter router)
        {
            router.Route<ReplenishBalanceCommand>()
                  .ToAggregate<MoneyBalance>()
            .Register();

            router.Route<WithdrawalBalanceCommand>()
                  .ToAggregate<MoneyBalance>()
             .Register();

            router.Route<CreateBalanceCommand>()
                  .ToAggregate<MoneyBalance>()
             .Register();


            router.Route<BalanceReplenishEvent>()
                    .ToHandler<BusinessCurrentBalanceProjectionBuilder>()
                    .WithCorrelation(nameof(BalanceReplenishEvent.BalanceId))
                  .Register();

            router.Route<BalanceCreatedEvent>()
                     .ToHandler<BusinessCurrentBalanceProjectionBuilder>()
                      .WithCorrelation(nameof(BalanceCreatedEvent.BalanceId))
                   .Register();

            router.Route<BalanceWithdrawalEvent>()
                   .ToHandler<BusinessCurrentBalanceProjectionBuilder>()
                    .WithCorrelation(nameof(BalanceWithdrawalEvent.BalanceId))
                .Register();
        }
    }

}