using BusinessNews.Domain.AccountAggregate;
using BusinessNews.Domain.AccountAggregate.Events;
using BusinessNews.Domain.BillAggregate;
using BusinessNews.Domain.BusinessAggregate;
using BusinessNews.Domain.Sagas.BuySubscription;
using BusinessNews.Domain.SubscriptionAggregate;
using BusinessNews.ReadModel;
using GridDomain.CQRS.Messaging;
using GridDomain.CQRS.Messaging.MessageRouting;

namespace BusinessNews.Node
{
    public class BusinessNewsRouting : IMessageRouteMap
    {
        public void Register(IMessagesRouter router)
        {
            router.RegisterAggregate<Account, AccountAggregateCommandsHandler>();
            router.RegisterAggregate<Bill, BillAggregateCommandsHandler>();
            router.RegisterAggregate<Subscription, SubscriptionAggregateCommandsHandler>();
            router.RegisterAggregate<Business, BusinessAggregateCommandsHandler>();

            router.RegisterSaga(BuySubscriptionSaga.Descriptor);

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