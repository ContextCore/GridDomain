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
    

    public class ClusterInternalMessagesSerializerConfig : IHoconConfig
    {
        public string Build()
        {
//            {  akka : {
//                    cluster : {
//                        singleton : {
//                            singleton-name : singleton
//                            role : 
//                            hand-over-retry-interval : 1s
//                            min-number-of-hand-over-retries : 10
//                        }
//                        singleton-proxy : {
//                            singleton-name : singleton
//                            role : 
//                            singleton-identification-interval : 1s
//                            buffer-size : 1000
//                        }
//                    }
//                    actor : {
//                        serializers : {
//                            akka-singleton : "Akka.Cluster.Tools.Singleton.Serialization.ClusterSingletonMessageSerializer, Akka.Cluster.Tools"
//                        }
//                        serialization-bindings : {
//                            "Akka.Cluster.Tools.Singleton.IClusterSingletonMessage, Akka.Cluster.Tools" : akka-singleton
//                        }
//                        serialization-identifiers : {
//                            "Akka.Cluster.Tools.Singleton.Serialization.ClusterSingletonMessageSerializer, Akka.Cluster.Tools" : 14
//                        }
//                    }
//                }
//            }
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
}