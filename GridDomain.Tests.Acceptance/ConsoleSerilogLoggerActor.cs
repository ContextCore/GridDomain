using GridDomain.Node;
using GridDomain.Node.Actors.Serilog;
using GridDomain.Node.Configuration;
using GridDomain.Node.Persistence.Sql;

namespace GridDomain.Tests.Acceptance {

    public static class NodeConfigurationDebugExtensions
    {
        public static string ToDebugStandAloneSystemConfig(this AkkaConfiguration conf)
        {
#if DEBUG
            return conf.ToStandAloneSystemConfig(true);
#else
            return conf.ToStandAloneSystemConfig(false);
#endif
        }
        
    }

    public class ConsoleSerilogLoggerActor : SerilogLoggerActor
    {
        public ConsoleSerilogLoggerActor():base(new DefaultLoggerConfiguration().CreateLogger())
        {
            
        }
    }
}