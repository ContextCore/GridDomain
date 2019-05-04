using GridDomain.Node.Akka.Configuration.Hocon;

namespace GridDomain.Node.Akka.Cluster.Hocon {
    public class ClusterActorProviderConfig : IHoconConfig
    {
        public string Build()
        {
            return @"akka.actor.provider = ""Akka.Cluster.ClusterActorRefProvider, Akka.Cluster""";
        }
    }
}