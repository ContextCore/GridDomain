using System;
using GridDomain.Balance.Domain.BalanceAggregate;
using GridDomain.Balance.ReadModel;
using GridDomain.CQRS.Messaging.MessageRouting;
using GridDomain.CQRS.ReadModel;
using GridDomain.Node.AkkaMessaging;
using GridDomain.Node.Configuration;
using Microsoft.Practices.Unity;

namespace GridDomain.Balance.Node
{
    public static class CompositionRoot
    {
        public static void Init(IUnityContainer container,
            IDbConfiguration conf)
        {
            //register all message handlers available to communicate
            //need to do it on plugin approach
            container.RegisterType<BalanceCommandsHandler>();

            Func<BusinessBalanceContext> contextFactory =
                () => new BusinessBalanceContext(conf.ReadModelConnectionString);

            container.RegisterType<IReadModelCreator<BusinessBalance>>(
                new InjectionFactory(c =>
                    new ReadModelCreatorRetryDecorator<BusinessBalance>(
                        new SqlReadModelCreator<BusinessBalance>(contextFactory))));

            container.RegisterType<IReadModelCreator<TransactionHistory>>(
                new InjectionFactory(c =>
                    new ReadModelCreatorRetryDecorator<TransactionHistory>(
                        new SqlReadModelCreator<TransactionHistory>(contextFactory))));

            container.RegisterType<BusinessCurrentBalanceProjectionBuilder>();
            container.RegisterType<TransactionsProjectionBuilder>();

            container.RegisterType<AggregateActor<MoneyBalance>>();
            container.RegisterType<AggregateHostActor<MoneyBalance>>();
            container.RegisterType<ICommandAggregateLocator<MoneyBalance>, MoneyBalanceCommandsHandler>();
            container.RegisterType<IAggregateCommandsHandler<MoneyBalance>, MoneyBalanceCommandsHandler>();
        }
    }
}