using System;
using Akka.Actor;
using Autofac;

namespace GridDomain.Scheduling.Akka {
    public static class AggregatesExtensions
    {

        public static AggregatesExtension GetAggregatesExtension(this ActorSystem sys)
        {
            return sys.GetExtension<AggregatesExtension>();
        }

            
        public static AggregatesExtension InitAggregatesExtension(this ActorSystem system,
                                                                  IContainer container)
        {
            if(system == null)
                throw new ArgumentNullException(nameof(system));

            return (AggregatesExtension)system.RegisterExtension(new AggregatesExtensionProvider(container));
        }
    }
}