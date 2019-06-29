using System;
using System.Linq;
using Akka.Actor;
using GridDomain.Aggregates.Abstractions;
using GridDomain.Domains;
using GridDomain.Node.Akka.Extensions.Aggregates;

namespace GridDomain.Node.Akka.Extensions.GridDomain {
    public static class AggregatesExtensions
    {

        public static GridDomainNodeExtension GetGridDomainExtension(this ActorSystem sys)
        {
            return sys.GetExtension<GridDomainNodeExtension>();
        }

            
        public static GridDomainNodeExtension InitGridDomainExtension(this ActorSystem system)
        {
            if(system == null)
                throw new ArgumentNullException(nameof(system));

            return (GridDomainNodeExtension)system.RegisterExtension(new GridDomainExtensionProvider());
        }
        
        public static INode InitGridDomainExtension(this ActorSystem system,
                                                    params IAggregatesDomainConfiguration[] configurations)
        {
            
            var gridDomainExtension = system.InitGridDomainExtension();
            
            gridDomainExtension.Register<IAggregatesDomainBuilder>(system.InitAggregatesExtension());
            gridDomainExtension.Add(configurations.Select(c => new AggregateDomainConfigurationAdapter(c)).ToArray());

            return gridDomainExtension.Build();
        }
    }
}