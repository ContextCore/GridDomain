using System;
using Akka.Actor;
using Autofac;
using GridDomain.Domains;

namespace GridDomain.Node.Akka.GridDomainNodeExtension {
    public static class AggregatesExtensions
    {

        public static GridDomainNode GetGridDomainExtension(this ActorSystem sys)
        {
            return sys.GetExtension<GridDomainNode>();
        }

            
        public static GridDomainNode InitGridDomainExtension(this ActorSystem system,
                                                                  params IDomainConfiguration[] configurations)
        {
            if(system == null)
                throw new ArgumentNullException(nameof(system));

            return (GridDomainNode)system.RegisterExtension(new GridDomainExtensionProvider(configurations));
        }
    }
}