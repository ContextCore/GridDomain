using System;
using Akka.Actor;
using Autofac;

namespace GridDomain.Node.Akka.Extensions.Aggregates {
    public static class AggregatesExtensions
    {

        public static AggregatesDomainExtension GetAggregatesExtension(this ActorSystem sys)
        {
            return sys.GetExtension<AggregatesDomainExtension>();
        }
            
        public static AggregatesDomainExtension InitAggregatesExtension(this ActorSystem system, ContainerBuilder builder=null)
        {
            if(system == null)
                throw new ArgumentNullException(nameof(system));

            return (AggregatesDomainExtension)system.RegisterExtension(new AggregatesExtensionProvider(builder));
        }
    }
}