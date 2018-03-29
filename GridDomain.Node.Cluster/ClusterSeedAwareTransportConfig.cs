using System;
using System.Linq;
using GridDomain.Node.Configuration.Hocon;

namespace GridDomain.Node.Cluster
{
    public class ClusterSeedAwareTransportConfig : IHoconConfig
    {
        private readonly string[] _seedNodeFullAddresses;

        public ClusterSeedAwareTransportConfig(params string[] seedNodeFullAddresses)
        {
            _seedNodeFullAddresses = seedNodeFullAddresses;
        }

        public string Build()
        {
            var seeds = string.Join(Environment.NewLine, _seedNodeFullAddresses.Select(n => @"""" + n + @""""));

            var clusterConfigString = @"actor.provider = ""Akka.Cluster.ClusterActorRefProvider, Akka.Cluster""";
            if (seeds.Any())
            {
                clusterConfigString +=
                @"
                cluster {
                    seed-nodes =  [" + seeds + @"]
                }
                ";
            }

            return clusterConfigString;
        }
     
    }
    

    public class ClusterSingletonInternalMessagesSerializerConfig : IHoconConfig
    {
        public string Build()
        {
            return @"actor : {
                                 serializers : {
                                     akka-singleton : ""Akka.Cluster.Tools.Singleton.Serialization.ClusterSingletonMessageSerializer, Akka.Cluster.Tools""
                                 }
                                 serialization-bindings : {
                                     ""Akka.Cluster.Tools.Singleton.IClusterSingletonMessage, Akka.Cluster.Tools"" : akka-singleton
                                 }
                                 serialization-identifiers : {
                                     ""Akka.Cluster.Tools.Singleton.Serialization.ClusterSingletonMessageSerializer, Akka.Cluster.Tools"" : 13
                                 }
                             }
                    ";
        }
    } 
    
    public class ClusterShardingMessagesSerializerConfig : IHoconConfig
    {
        public string Build()
        {
            return @"actor : {
                                 serializers : {
                                      akka-sharding = ""Akka.Cluster.Sharding.Serialization.ClusterShardingMessageSerializer, Akka.Cluster.Sharding""
                                 }
                                 serialization-bindings : {
                                     ""Akka.Cluster.Sharding.IClusterShardingSerializable, Akka.Cluster.Sharding"" = akka-sharding
                                 }
                                 serialization-identifiers : {
                                     ""Akka.Cluster.Sharding.Serialization.ClusterShardingMessageSerializer, Akka.Cluster.Sharding"" = 13
                                 }
                             }
                    ";
        }
    }
    
    public class HyperionForAll : IHoconConfig
    {
        public string Build()
        {
            return @"actor : {
                                 serializers : {
                                     hyperion = ""Akka.Serialization.HyperionSerializer, Akka.Serialization.Hyperion""
                                 }
                                 serialization-bindings : {
                                     ""System.Object"" = hyperion
                                 }
                             }
                    ";
        }
    }
}