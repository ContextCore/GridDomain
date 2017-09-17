using GridDomain.Node;
using GridDomain.Node.Actors.Serilog;
using GridDomain.Tests.Common;
using Serilog;
using Serilog.Events;

namespace GridDomain.Tests.Acceptance.Tools {
    public class DefaultConsoleLoggerConfiguration : DefaultLoggerConfiguration
    {
        public DefaultConsoleLoggerConfiguration(LogEventLevel level = LogEventLevel.Verbose) : base(level)
        {
            WriteTo.Console();
        }
    }

    public class ConsoleSerilogLoggerActor : SerilogLoggerActor
    {
        public ConsoleSerilogLoggerActor():base(new DefaultLoggerConfiguration())
        {
            
        }
    }
}