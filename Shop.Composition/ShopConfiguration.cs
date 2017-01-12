using System;
using GridDomain.Common;
using GridDomain.Node.Actors;
using GridDomain.Node.Configuration.Composition;
using Microsoft.EntityFrameworkCore;
using Microsoft.Practices.Unity;
using Shop.Domain.Aggregates.AccountAggregate;
using Shop.Domain.Aggregates.OrderAggregate;
using Shop.Domain.Aggregates.SkuAggregate;
using Shop.Domain.Aggregates.SkuStockAggregate;
using Shop.Domain.Aggregates.UserAggregate;
using Shop.Domain.DomainServices.PriceCalculator;
using Shop.Domain.Sagas;
using Shop.Infrastructure;
using Shop.ReadModel.Context;
using Shop.ReadModel.DomanServices;
using Account = Shop.Domain.Aggregates.AccountAggregate.Account;
using Order = Shop.Domain.Aggregates.OrderAggregate.Order;
using Sku = Shop.Domain.Aggregates.SkuAggregate.Sku;
using SkuStock = Shop.Domain.Aggregates.SkuStockAggregate.SkuStock;
using User = Shop.Domain.Aggregates.UserAggregate.User;

namespace Shop.Composition
{
    public class ShopConfiguration : IContainerConfiguration
    {
        public void Register(IUnityContainer container)
        {
            var contextSqlOptions = new DbContextOptionsBuilder<ShopDbContext>()
                                    .UseSqlServer("Server = (local); Database = Shop; Integrated Security = true; MultipleActiveResultSets = True")
                                    .Options;

            container.RegisterInstance<Func<ShopDbContext>>(() => new ShopDbContext(contextSqlOptions));
            container.RegisterType<ISkuPriceQuery, SkuPriceQuery>(new ContainerControlledLifetimeManager());
            container.RegisterType<IPriceCalculator, SqlPriceCalculator>(new ContainerControlledLifetimeManager());

            container.Register(new AggregateConfiguration<Account,AccountCommandsHandler>());
            container.Register(new AggregateConfiguration<Order,OrderCommandsHandler>());
            container.Register(new AggregateConfiguration<Sku,SkuCommandsHandler>());
            container.Register(new AggregateConfiguration<SkuStock,SkuStockCommandsHandler>());
            container.Register(new AggregateConfiguration<User,UserCommandsHandler>());
            container.RegisterType<ISequenceProvider,SqlSequenceProvider>();
            container.RegisterType<IPriceCalculator,SqlPriceCalculator>();
            container.Register(SagaConfiguration.Instance<BuyNow, BuyNowData, BuyNowSagaFactory>(BuyNow.Descriptor));
        }
    }
}