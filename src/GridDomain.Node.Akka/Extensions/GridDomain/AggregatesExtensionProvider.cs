using Akka.Actor;
using GridDomain.Domains;

namespace GridDomain.Node.Akka.Extensions.GridDomain {
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