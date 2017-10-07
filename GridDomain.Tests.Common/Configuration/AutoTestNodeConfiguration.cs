using Akka.Event;
using GridDomain.Node.Configuration;
using Serilog.Events;

namespace GridDomain.Tests.Common.Configuration
{
    public class AutoTestNodeConfiguration : NodeConfiguration
    {
        public static NodeConfiguration Default { get; } = new AutoTestNodeConfiguration();
        public AutoTestNodeConfiguration(LogEventLevel verbosity = LogEventLevel.Information)
            : base("AutoTest",new AutoTestNodeNetworkAddress(), verbosity) {}
    }
}