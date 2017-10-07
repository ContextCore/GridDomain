using Akka.Event;
using GridDomain.Node.Configuration;
using GridDomain.Node.Persistence.Sql;
using Serilog.Events;

namespace GridDomain.Tests.Acceptance {
    public static class NodeConfigurationDebugExtensions
    {
        public static string ToDebugStandAloneSystemConfig(this NodeConfiguration conf, ISqlNodeDbConfiguration persistence)
        {
#if DEBUG
            return conf.ToStandAloneSystemConfig(persistence, true);
#else
            //to reduce produced logs 
            conf.LogLevel = LogEventLevel.Warning;
            return conf.ToStandAloneSystemConfig(persistence);
#endif
        }

    }
}