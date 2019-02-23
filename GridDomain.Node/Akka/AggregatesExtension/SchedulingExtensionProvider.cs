using Akka.Actor;
using Autofac;

namespace GridDomain.Node.Akka.AggregatesExtension {
    public class AggregatesExtensionProvider : ExtensionIdProvider<AggregatesExtension>
    {
        private readonly IContainer _container;

        public AggregatesExtensionProvider(IContainer container)
        {
            _container = container;
        }

        public override AggregatesExtension CreateExtension(ExtendedActorSystem system)
        {
            return new AggregatesExtension(_container);
        }
    }
}