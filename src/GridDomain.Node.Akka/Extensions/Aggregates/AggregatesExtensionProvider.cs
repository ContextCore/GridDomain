using Akka.Actor;
using Autofac;

namespace GridDomain.Node.Akka.Extensions.Aggregates {
    public class AggregatesExtensionProvider : ExtensionIdProvider<AggregatesExtension>
    {
        private readonly ContainerBuilder _builder;

        public AggregatesExtensionProvider(ContainerBuilder builder = null)
        {
            _builder = builder;
        }
        public override AggregatesExtension CreateExtension(ExtendedActorSystem system)
        {
            return new AggregatesExtension(system,_builder);
        }
    }
}