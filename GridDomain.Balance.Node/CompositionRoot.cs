using System;
using BusinessNews.Domain.AccountAggregate;
using BusinessNews.Domain.BillAggregate;
using BusinessNews.Domain.BusinessAggregate;
using BusinessNews.Domain.Sagas.BuySubscription;
using BusinessNews.Domain.SubscriptionAggregate;
using BusinessNews.ReadModel;
using GridDomain.CQRS.Messaging.MessageRouting;
using GridDomain.CQRS.ReadModel;
using GridDomain.EventSourcing.Sagas;
using GridDomain.Node.Actors;
using GridDomain.Node.Configuration;
using GridDomain.Node.Configuration.Composition;
using GridDomain.Node.Configuration.Persistence;
using Microsoft.Practices.Unity;

namespace BusinessNews.Node
{
    public static class CompositionRoot
    {
        public static void Init(IUnityContainer container,
                                IDbConfiguration conf)
        {
            RegisterReadModel(container, conf);

            RegisterSaga_BuySubscription(container);

            container.RegisterAggregate<Account, AccountAggregateCommandsHandler>();
            container.RegisterAggregate<Bill, BillAggregateCommandsHandler>();
            container.RegisterAggregate<Business, BusinessAggregateCommandsHandler>();
            container.RegisterAggregate<Subscription, SubscriptionAggregateCommandsHandler>();
        }

        private static void RegisterSaga_BuySubscription(IUnityContainer container)
        {

            container
                .RegisterType<ISagaFactory<BuySubscriptionSaga, BuySubscriptionSagaStateAggregate>, BuySubscriptionSagaFactory>();
            container.RegisterType<ISagaFactory<BuySubscriptionSaga, SubscriptionOrderedEvent>, BuySubscriptionSagaFactory>();
            container.RegisterType<IEmptySagaFactory<BuySubscriptionSaga>, BuySubscriptionSagaFactory>();
        }

        private static void RegisterReadModel(IUnityContainer container, IDbConfiguration conf)
        {
            Func<BusinessBalanceContext> contextFactory = () => new BusinessBalanceContext(conf.ReadModelConnectionString);

            container.RegisterType<IReadModelCreator<BusinessAccount>>(
                new InjectionFactory(c =>
                    new ReadModelCreatorRetryDecorator<BusinessAccount>(
                        new SqlReadModelCreator<BusinessAccount>(contextFactory))));
            container.RegisterType<BusinessCurrentBalanceProjectionBuilder>();


            container.RegisterType<IReadModelCreator<TransactionHistory>>(
                new InjectionFactory(c =>
                    new ReadModelCreatorRetryDecorator<TransactionHistory>(
                        new SqlReadModelCreator<TransactionHistory>(contextFactory))));
            container.RegisterType<TransactionsProjectionBuilder>();
        }
    }
}