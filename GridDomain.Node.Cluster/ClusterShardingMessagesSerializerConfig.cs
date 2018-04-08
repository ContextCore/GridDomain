using GridDomain.Node.Configuration.Hocon;

namespace GridDomain.Node.Cluster {
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
}