using Akka.Event;
using GridDomain.Node.Configuration;
using GridDomain.Tests.Acceptance;
using GridDomain.Tests.Common;
using GridDomain.Tests.Common.Configuration;
using Serilog.Events;

namespace GridDomain.Tests.Stress
{
    public class StressTestNodeConfiguration : NodeConfiguration
    {
        public StressTestNodeConfiguration(LogEventLevel verbosity = LogEventLevel.Warning)
            : base("StressTest",new StressTestNodeNetworkAddress(), verbosity) {}
    }
}