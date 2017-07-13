using GridDomain.Configuration;
using GridDomain.Node.Configuration.Composition;
using Shop.Domain.Aggregates.SkuAggregate;
using Shop.Infrastructure;

namespace Shop.Composition {
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
}