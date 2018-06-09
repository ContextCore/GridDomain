using Akka.Configuration;
using GridDomain.Node.Configuration.Hocon;

namespace GridDomain.Node.Cluster.Configuration.Hocon {
    public class AutoTerminateProcessOnClusterShutdown : IHoconConfig
    {
        public Config Build()
        {
            return "coordinated-shutdown.exit-clr = on";
        }
    }
}