using Akka.Actor;
using Autofac;

namespace GridDomain.Scheduling.Akka {
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