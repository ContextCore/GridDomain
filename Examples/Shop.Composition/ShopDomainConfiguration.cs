using System;
using GridDomain.Node.Configuration.Composition;
using Microsoft.EntityFrameworkCore;
using Microsoft.Practices.Unity;
using Shop.Domain.DomainServices.PriceCalculator;
using Shop.Infrastructure;
using Shop.ReadModel.Context;
using Shop.ReadModel.DomanServices;

namespace Shop.Composition {
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
}