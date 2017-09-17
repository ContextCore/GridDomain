using Akka.Event;
using GridDomain.Node.Actors.Serilog;
using GridDomain.Node.Configuration.Akka;
using GridDomain.Tests.Common;
using GridDomain.Tests.Common.Configuration;

namespace GridDomain.Tests.Stress
{
    public class StressTestAkkaConfiguration : AkkaConfiguration
    {
        public StressTestAkkaConfiguration(LogLevel verbosity = LogLevel.WarningLevel)
            : base(new StressTestAkkaNetworkAddress(), new AutoTestAkkaDbConfiguration(), typeof(SerilogLoggerActor), verbosity) {}
    }
}