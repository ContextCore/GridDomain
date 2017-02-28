using Akka.Event;
using GridDomain.Node.Configuration.Akka;
using GridDomain.Tests.Framework.Configuration;

namespace GridGomain.Tests.Stress
{
    public class StressTestAkkaConfiguration : AkkaConfiguration
    {
        public StressTestAkkaConfiguration(LogLevel verbosity = LogLevel.DebugLevel)
            : base(new StressTestAkkaNetworkAddress(), new AutoTestAkkaDbConfiguration(), verbosity) {}
    }
}