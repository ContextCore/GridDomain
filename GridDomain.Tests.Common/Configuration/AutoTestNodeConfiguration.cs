using Akka.Event;
using GridDomain.Node.Actors.Serilog;
using GridDomain.Node.Configuration;

namespace GridDomain.Tests.Common.Configuration
{
    public class AutoTestAkkaConfiguration : AkkaConfiguration
    {
        public AutoTestAkkaConfiguration(LogLevel verbosity = LogLevel.DebugLevel)
            : base(new AutoTestNodeNetworkAddress(), new AutoTestNodeDbConfiguration(), verbosity) {}
    }
}