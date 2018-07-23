using System.Linq;
using GridDomain.Node.Configuration;

namespace GridDomain.Node.Cluster.Configuration {


    public static class ClusterConfigBuilderExtensions
    {
       
    
        public static ClusterConfigBuilder Seeds(this ClusterConfigBuilder builder, params int[] ports)
        {
            builder.Seeds(ports.Select(p => new NodeNetworkAddress(null, p))
                               .Cast<INodeNetworkAddress>()
                               .ToArray());
            return builder;
        }
        
        public static ClusterConfigBuilder Workers(this ClusterConfigBuilder builder, int number)
        {
            builder.Workers(Enumerable.Range(0,number)
                                      .Select(p => new NodeNetworkAddress())
                                      .Cast<INodeNetworkAddress>()
                                      .ToArray());
            return builder;
            
        }

       
        public static ClusterConfigBuilder AutoSeeds(this ClusterConfigBuilder builder, int number)
        {
           builder.Seeds(Enumerable.Range(0, number)
                                      .Select(p => new NodeNetworkAddress())
                                      .Cast<INodeNetworkAddress>()
                                      .ToArray());
            return builder;
        }


       
    }
}