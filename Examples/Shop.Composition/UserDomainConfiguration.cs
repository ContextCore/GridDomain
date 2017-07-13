using GridDomain.Configuration;
using GridDomain.Node.Configuration.Composition;
using Shop.Domain.Aggregates.UserAggregate;

namespace Shop.Composition {
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
}