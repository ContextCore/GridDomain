using GridDomain.Balance.Domain.AccountAggregate;
using GridDomain.Balance.Domain.AccountAggregate.Events;
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
            router.Register<Account, AccountAggregateCommandsHandler>(new AccountAggregateCommandsHandler());

            router.Route<AccountBalanceReplenishEvent>()
                .ToHandler<BusinessCurrentBalanceProjectionBuilder>()
                .WithCorrelation(nameof(AccountBalanceReplenishEvent.BalanceId))
                .Register();

            router.Route<AccountCreatedEvent>()
                .ToHandler<BusinessCurrentBalanceProjectionBuilder>()
                .WithCorrelation(nameof(AccountCreatedEvent.BalanceId))
                .Register();

            router.Route<PayedForBillEvent>()
                .ToHandler<BusinessCurrentBalanceProjectionBuilder>()
                .WithCorrelation(nameof(PayedForBillEvent.BalanceId))
                .Register();

            router.Route<AccountBalanceReplenishEvent>()
                .ToHandler<TransactionsProjectionBuilder>()
                .WithCorrelation(nameof(AccountBalanceReplenishEvent.BalanceId))
                .Register();

            router.Route<AccountCreatedEvent>()
                .ToHandler<TransactionsProjectionBuilder>()
                .WithCorrelation(nameof(AccountCreatedEvent.BalanceId))
                .Register();

            router.Route<PayedForBillEvent>()
                .ToHandler<TransactionsProjectionBuilder>()
                .WithCorrelation(nameof(PayedForBillEvent.BalanceId))
                .Register();
        }
    }
}