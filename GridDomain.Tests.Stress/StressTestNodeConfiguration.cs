using Akka.Event;
using GridDomain.Node.Actors.Serilog;
using GridDomain.Node.Configuration;
using GridDomain.Tests.Common;
using GridDomain.Tests.Common.Configuration;

namespace GridDomain.Tests.Stress
{
    public class StressTestNodeConfiguration : NodeConfiguration
    {
        public StressTestNodeConfiguration(LogLevel verbosity = LogLevel.WarningLevel)
            : base(new StressTestNodeNetworkAddress(), new AutoTestNodeDbConfiguration(), verbosity) {}
    }
}