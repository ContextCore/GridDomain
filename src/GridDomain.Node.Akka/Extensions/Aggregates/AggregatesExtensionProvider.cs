using Akka.Actor;
using Autofac;

namespace GridDomain.Node.Akka.Extensions.Aggregates {
    public class AggregatesExtensionProvider : ExtensionIdProvider<AggregatesDomainExtension>
    {
        private readonly ContainerBuilder _builder;

        public AggregatesExtensionProvider(ContainerBuilder builder = null)
        {
            _builder = builder;
        }
        public override AggregatesDomainExtension CreateExtension(ExtendedActorSystem system)
        {
            return new AggregatesDomainExtension(system,_builder);
        }
    }
}