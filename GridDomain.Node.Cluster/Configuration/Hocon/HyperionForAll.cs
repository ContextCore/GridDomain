using Akka.Configuration;
using GridDomain.Node.Configuration.Hocon;

namespace GridDomain.Node.Cluster.Configuration.Hocon {
    public class HyperionForAll : IHoconConfig
    {
        public string Build()
        {
            return @"akka.actor : {
                                 serializers : {
                                     hyperion = ""GridDomain.Node.DebugHyperionSerializer, GridDomain.Node""
                                 }
                                 serialization-bindings : {
                                     ""System.Object"" = hyperion
                                 }
                             }
                    ";
        }
    }
}