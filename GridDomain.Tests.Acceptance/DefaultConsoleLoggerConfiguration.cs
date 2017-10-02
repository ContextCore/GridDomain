using GridDomain.Node;
using Serilog;
using Serilog.Events;

namespace GridDomain.Tests.Acceptance {
    public class DefaultConsoleLoggerConfiguration : DefaultLoggerConfiguration
    {
        public DefaultConsoleLoggerConfiguration(LogEventLevel level = LogEventLevel.Verbose) : base(level)
        {
            WriteTo.Console();
        }
    }
}