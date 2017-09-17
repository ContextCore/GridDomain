using Akka.Event;
using GridDomain.Node.Actors.Serilog;
using GridDomain.Node.Configuration.Akka;

namespace GridDomain.Tests.Common.Configuration
{
    public class AutoTestAkkaConfiguration : AkkaConfiguration
    {
        public AutoTestAkkaConfiguration(LogLevel verbosity = LogLevel.DebugLevel)
            : base(new AutoTestAkkaNetworkAddress(), new AutoTestAkkaDbConfiguration(), typeof(SerilogLoggerActor), verbosity) {}
    }
}