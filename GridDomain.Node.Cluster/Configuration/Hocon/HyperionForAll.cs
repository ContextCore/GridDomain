using Akka.Configuration;
using GridDomain.Node.Configuration.Hocon;

namespace GridDomain.Node.Cluster.Configuration.Hocon {
    public class HyperionForAll : IHoconConfig
    {
        public string Build()
        {
            return @"akka.actor : {
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