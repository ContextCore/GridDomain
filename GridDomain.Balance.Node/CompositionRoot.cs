using System;
using BusinessNews.Domain.AccountAggregate;
using BusinessNews.Domain.BillAggregate;
using BusinessNews.Domain.BusinessAggregate;
using BusinessNews.Domain.OfferAggregate;
using BusinessNews.Domain.Sagas.BuySubscription;
using BusinessNews.Domain.SubscriptionAggregate;
using BusinessNews.ReadModel;
using GridDomain.CQRS.Messaging.MessageRouting;
using GridDomain.CQRS.ReadModel;
using GridDomain.EventSourcing.Sagas;
using GridDomain.Node.Actors;
using GridDomain.Node.Configuration;
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

            RegisterAggregate_Account(container);

            RegisterAggregate_Bill(container);

            RegisterAggregate_Business(container);

            RegisterAggregate_Subscription(container);
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

        private static void RegisterAggregate_Subscription(IUnityContainer container)
        {
            container.RegisterType<AggregateActor<Subscription>>();
            container.RegisterType<AggregateHubActor<Subscription>>();
            container.RegisterType<ICommandAggregateLocator<Subscription>, SubscriptionAggregateCommandsHandler>();
            container.RegisterType<IAggregateCommandsHandler<Subscription>, SubscriptionAggregateCommandsHandler>();
        }

        private static void RegisterAggregate_Business(IUnityContainer container)
        {
            container.RegisterType<AggregateActor<Business>>();
            container.RegisterType<AggregateHubActor<Business>>();
            container.RegisterType<ICommandAggregateLocator<Business>, BusinessAggregateCommandsHandler>();
            container.RegisterType<IAggregateCommandsHandler<Business>, BusinessAggregateCommandsHandler>();
        }

        private static void RegisterAggregate_Bill(IUnityContainer container)
        {
            container.RegisterType<AggregateActor<Bill>>();
            container.RegisterType<AggregateHubActor<Bill>>();
            container.RegisterType<ICommandAggregateLocator<Bill>, BillAggregateCommandsHandler>();
            container.RegisterType<IAggregateCommandsHandler<Bill>, BillAggregateCommandsHandler>();
        }

        private static void RegisterAggregate_Account(IUnityContainer container)
        {
            container.RegisterType<AggregateActor<Account>>();
            container.RegisterType<AggregateHubActor<Account>>();
            container.RegisterType<ICommandAggregateLocator<Account>, AccountAggregateCommandsHandler>();
            container.RegisterType<IAggregateCommandsHandler<Account>, AccountAggregateCommandsHandler>();
        }
    }
}