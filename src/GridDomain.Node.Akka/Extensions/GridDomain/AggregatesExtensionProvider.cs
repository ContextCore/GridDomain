using System;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Akka.Actor;
using Autofac;
using GridDomain.Abstractions;
using GridDomain.Aggregates.Abstractions;
using GridDomain.Domains;

namespace GridDomain.Node.Akka.Extensions.GridDomain {
    public class GridDomainExtensionProvider : ExtensionIdProvider<GridDomainNodeExtension>
    {
        public override GridDomainNodeExtension CreateExtension(ExtendedActorSystem system)
        {
            return new GridDomainNodeExtension(system);
        }
    }

    public class AggregateDomainConfigurationAdapter : IDomainConfiguration
    {
        private readonly IAggregatesDomainConfiguration _cfg;

        public AggregateDomainConfigurationAdapter(IAggregatesDomainConfiguration cfg)
        {
            _cfg = cfg;
        }

        public async Task Configure(IDomainBuilder builder)
        { 
            await  _cfg.Register(builder.GetAggregatesBuilder());
        }
    }
}