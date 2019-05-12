using Akka.Actor;
using Autofac;
using GridDomain.Domains;

namespace GridDomain.Node.Akka.GridDomainNodeExtension {
    public class GridDomainExtensionProvider : ExtensionIdProvider<GridDomainNode>
    {
        private readonly IDomainConfiguration[] _domainConfigurations;

        public GridDomainExtensionProvider(params IDomainConfiguration[] domainConfigurations)
        {
            _domainConfigurations = domainConfigurations;
        }

        public override GridDomainNode CreateExtension(ExtendedActorSystem system)
        {
            return new GridDomainNode(system, _domainConfigurations);
        }
    }
}