using GridDomain.Node.Configuration.Hocon;

namespace GridDomain.Node.Cluster {
    public class PubSubConfig : IHoconConfig 
    {
        public string Build()
        {
            return @"extensions = [""Akka.Cluster.Tools.PublishSubscribe.DistributedPubSubExtensionProvider,Akka.Cluster.Tools""]";
        }
    }
}