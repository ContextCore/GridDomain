using Akka.Event;
using GridDomain.Node.Configuration;
using GridDomain.Node.Persistence.Sql;

namespace GridDomain.Tests.Acceptance {
    public static class NodeConfigurationDebugExtensions
    {
        public static string ToDebugStandAloneSystemConfig(this AkkaConfiguration conf, ISqlNodeDbConfiguration persistence)
        {
#if DEBUG
            return conf.ToStandAloneSystemConfig(persistence, true);
#else
            //to reduce produced logs 
            conf.LogLevel = LogLevel.WarningLevel;
            return conf.ToStandAloneSystemConfig(persistence,false);
#endif
        }

    }
}