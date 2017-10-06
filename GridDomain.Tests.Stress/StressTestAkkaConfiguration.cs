using Akka.Event;
using GridDomain.Node.Actors.Serilog;
using GridDomain.Node.Configuration;
using GridDomain.Tests.Acceptance;
using GridDomain.Tests.Common;
using GridDomain.Tests.Common.Configuration;

namespace GridDomain.Tests.Stress
{
    public class StressTestAkkaConfiguration : AkkaConfiguration
    {
        public StressTestAkkaConfiguration(LogLevel verbosity = LogLevel.WarningLevel)
            : base(new StressTestNodeNetworkAddress(), verbosity) {}
    }
}