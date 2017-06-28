using GridDomain.Node.Configuration.Composition;
using Shop.Domain.Aggregates.SkuStockAggregate;

namespace Shop.Composition {
    class SkuStockDomainConfiguration : IDomainConfiguration {
        public void Register(IDomainBuilder builder)
        {
            builder.RegisterAggregate(DefaultAggregateDependencyFactory.New(new SkuStockCommandsHandler()));

        }
    }
}