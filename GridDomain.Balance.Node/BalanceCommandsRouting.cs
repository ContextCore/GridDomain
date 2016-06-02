using GridDomain.Balance.Domain.BalanceAggregate;
using GridDomain.Balance.Domain.BalanceAggregate.Events;
using GridDomain.Balance.ReadModel;
using GridDomain.CQRS.Messaging;
using GridDomain.CQRS.Messaging.MessageRouting;
using GridDomain.Node.AkkaMessaging;

namespace GridDomain.Balance.Node
{
    public class BalanceCommandsRouting : IMessageRouteMap
    {
        public void Register(IMessagesRouter router)
        {
            router.Register<MoneyBalance, MoneyBalanceCommandsHandler>(new MoneyBalanceCommandsHandler());

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

            router.Route<BalanceReplenishEvent>()
                .ToHandler<TransactionsProjectionBuilder>()
                .WithCorrelation(nameof(BalanceReplenishEvent.BalanceId))
                .Register();

            router.Route<BalanceCreatedEvent>()
                .ToHandler<TransactionsProjectionBuilder>()
                .WithCorrelation(nameof(BalanceCreatedEvent.BalanceId))
                .Register();

            router.Route<BalanceWithdrawalEvent>()
                .ToHandler<TransactionsProjectionBuilder>()
                .WithCorrelation(nameof(BalanceWithdrawalEvent.BalanceId))
                .Register();

            //TransactionsProjectionBuilder
        }
    }
}