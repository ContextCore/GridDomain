using GridDomain.Configuration;
using GridDomain.Node.Configuration.Composition;
using Shop.Domain.Aggregates.OrderAggregate;
using Shop.Infrastructure;

namespace Shop.Composition {
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
}