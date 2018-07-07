using GridDomain.Node.Configuration.Hocon;

namespace GridDomain.Node.Cluster.Configuration.Hocon {
    public class ClusterActorProviderConfig : IHoconConfig
    {
        public string Build()
        {
            return @"akka.actor.provider = ""Akka.Cluster.ClusterActorRefProvider, Akka.Cluster""";
        }
    }
}