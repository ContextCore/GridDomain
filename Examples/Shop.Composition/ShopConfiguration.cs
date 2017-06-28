using System;
using GridDomain.Node.Configuration.Composition;
using Microsoft.EntityFrameworkCore;
using Microsoft.Practices.Unity;
using Serilog;
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

namespace Shop.Composition
{
    class AccountDomainConfiguration : IDomainConfiguration {
        public void Register(IDomainBuilder builder)
        {
            builder.RegisterAggregate(DefaultAggregateDependencyFactory.New<Account,AccountCommandsHandler>());
        }
    }
    class OrderDomainConfiguration : IDomainConfiguration {
        private readonly ISequenceProvider _sequenceProvider;

        public OrderDomainConfiguration(ISequenceProvider sequenceProvider)
        {
            _sequenceProvider = sequenceProvider;
        }
        public void Register(IDomainBuilder builder)
        {
            builder.RegisterAggregate(DefaultAggregateDependencyFactory.New(new OrderCommandsHandler(_sequenceProvider)));
        }
    }
    class SkuDomainConfiguration : IDomainConfiguration {
        private readonly ISequenceProvider _sequenceProvider;
        public SkuDomainConfiguration(ISequenceProvider sequenceProvider)
        {
            _sequenceProvider = sequenceProvider;
        }

        public void Register(IDomainBuilder builder)
        {
            builder.RegisterAggregate(DefaultAggregateDependencyFactory.New(new SkuCommandsHandler(_sequenceProvider)));
        }
    }
    class SkuStockDomainConfiguration : IDomainConfiguration {
        public void Register(IDomainBuilder builder)
        {
            builder.RegisterAggregate(DefaultAggregateDependencyFactory.New(new SkuStockCommandsHandler()));

        }
    }
    class UserDomainConfiguration : IDomainConfiguration {
        private readonly IDefaultStockProvider _stockProvider;
        public UserDomainConfiguration(IDefaultStockProvider stockProvider)
        {
            _stockProvider = stockProvider;
        }

        public void Register(IDomainBuilder builder)
        {
            builder.RegisterAggregate(DefaultAggregateDependencyFactory.New(new UserCommandsHandler(_stockProvider)));

        }
    }

    public class ShopDomainConfiguration : IDomainConfiguration
    {
        private readonly DbContextOptions<ShopDbContext> _readModelContextOptions;
        private readonly IUnityContainer _container;

        public ShopDomainConfiguration(DbContextOptions<ShopDbContext> readModelContextOptions = null)
        {
            _readModelContextOptions = readModelContextOptions
                                       ?? new DbContextOptionsBuilder<ShopDbContext>().UseSqlServer(
                                                                                                    "Server = (local); Database = Shop; Integrated Security = true; MultipleActiveResultSets = True")
                                                                                      .Options;
            _container = new UnityContainer();
        }

        private void Compose(UnityContainer container)
        {
            container.RegisterInstance<Func<ShopDbContext>>(() => new ShopDbContext(_readModelContextOptions));
            container.RegisterType<ISkuPriceQuery, SkuPriceQuery>(new ContainerControlledLifetimeManager());
            container.RegisterType<IPriceCalculator, SqlPriceCalculator>(new ContainerControlledLifetimeManager());
            container.RegisterType<ISequenceProvider, SqlSequenceProvider>();

            container.RegisterType<BuyNowSagaDomainConfiguration>(new ContainerControlledLifetimeManager());

            container.RegisterType<AccountDomainConfiguration>(new ContainerControlledLifetimeManager());
            container.RegisterType<SkuDomainConfiguration>(new ContainerControlledLifetimeManager());
            container.RegisterType<OrderDomainConfiguration>(new ContainerControlledLifetimeManager());
            container.RegisterType<SkuStockDomainConfiguration>(new ContainerControlledLifetimeManager());
            container.RegisterType<UserDomainConfiguration>(new ContainerControlledLifetimeManager());

        }
      
        public void Register(IDomainBuilder builder)
        {
            builder.Register(_container.Resolve<BuyNowSagaDomainConfiguration>());
            builder.Register(_container.Resolve<AccountDomainConfiguration>());
            builder.Register(_container.Resolve<SkuDomainConfiguration>());
            builder.Register(_container.Resolve<OrderDomainConfiguration>());
            builder.Register(_container.Resolve<SkuStockDomainConfiguration>());
            builder.Register(_container.Resolve<UserDomainConfiguration>());
        }
    }

    public class BuyNowSagaDomainConfiguration : IDomainConfiguration
    {
        private readonly IPriceCalculator _calculator;
        private readonly ILogger _log;
        public BuyNowSagaDomainConfiguration(IPriceCalculator calculator, ILogger log)
        {
            _calculator = calculator;
            _log = log;
        }

        public void Register(IDomainBuilder builder)
        {
            builder.RegisterSaga(new DefaultSagaDependencyFactory<BuyNow, BuyNowState>(new BuyNowSagaFactory(_calculator, _log), BuyNow.Descriptor));
        }
    }
}