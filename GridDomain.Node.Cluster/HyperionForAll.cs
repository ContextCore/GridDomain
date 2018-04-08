using GridDomain.Node.Configuration.Hocon;

namespace GridDomain.Node.Cluster {
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