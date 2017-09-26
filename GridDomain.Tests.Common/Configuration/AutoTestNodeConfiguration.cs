using Akka.Event;
using GridDomain.Node.Actors.Serilog;
using GridDomain.Node.Configuration;

namespace GridDomain.Tests.Common.Configuration
{
    public class AutoTestNodeConfiguration : NodeConfiguration
    {
        public AutoTestNodeConfiguration(LogLevel verbosity = LogLevel.DebugLevel)
            : base(new AutoTestNodeNetworkAddress(), new AutoTestNodeDbConfiguration(), typeof(SerilogLoggerActor), verbosity) {}
    }
}