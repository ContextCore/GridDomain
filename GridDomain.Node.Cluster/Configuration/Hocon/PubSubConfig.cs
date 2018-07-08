using Akka.Configuration;
using GridDomain.Node.Configuration.Hocon;

namespace GridDomain.Node.Cluster.Configuration.Hocon {
    public class PubSubConfig : IHoconConfig 
    {
        public string Build()
        {
            return @"akka.extensions = [""Akka.Cluster.Tools.PublishSubscribe.DistributedPubSubExtensionProvider,Akka.Cluster.Tools""]";
        }
    }
}