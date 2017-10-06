using Akka.Event;
using GridDomain.Node.Configuration;

namespace GridDomain.Tests.Common.Configuration
{
    public class AutoTestAkkaConfiguration : AkkaConfiguration
    {
        public static AkkaConfiguration Default { get; } = new AutoTestAkkaConfiguration();
        public AutoTestAkkaConfiguration(LogLevel verbosity = LogLevel.DebugLevel)
            : base(new AutoTestNodeNetworkAddress(), verbosity) {}
    }
}