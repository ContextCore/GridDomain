using GridDomain.Node.Configuration.Hocon;

namespace GridDomain.Node.Cluster {
    public class AutoTerminateProcessOnClusterShutdown : IHoconConfig
    {
        public string Build()
        {
            return "coordinated-shutdown.exit-clr = on";
        }
    }
}