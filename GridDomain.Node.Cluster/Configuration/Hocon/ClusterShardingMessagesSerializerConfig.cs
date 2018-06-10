using Akka.Configuration;
using GridDomain.Node.Configuration.Hocon;

namespace GridDomain.Node.Cluster.Configuration.Hocon {
    public class ClusterShardingMessagesSerializerConfig : IHoconConfig
    {
        public Config Build()
        {
            return @"akka.actor : {
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
}