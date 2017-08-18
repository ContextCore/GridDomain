using Akka.Event;
using GridDomain.Node.Configuration.Akka;
using GridDomain.Tests.Common.Configuration;

namespace GridGomain.Tests.Stress
{
    public class StressTestAkkaConfiguration : AkkaConfiguration
    {
        public StressTestAkkaConfiguration(LogLevel verbosity = LogLevel.WarningLevel)
            : base(new StressTestAkkaNetworkAddress(), new AutoTestAkkaDbConfiguration(), verbosity) {}
    }
}