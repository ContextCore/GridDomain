using Akka.Remote.Transport;
using GridDomain.Logging;
using Serilog;
using Serilog.Events;
using ILogger = GridDomain.Logging.ILogger;

namespace GridDomain.Tests.Framework
{
    public class AutoTestLogFactory : LoggerFactory
    {
        public override ILogger GetLogger(string className = null)
        {
            className = className ?? GetClassName();
            return new SerilogLogger(GetConfiguration().CreateLogger()).ForContext("className", className);
        }

        private static LoggerConfiguration GetConfiguration()
        {
            return new LoggerConfiguration().WriteTo.RollingFile(".\\GridDomainLogs\\logs-{yyyy-MM-dd_HH_mm_ss}}.txt")
                                            .WriteTo.Console(LogEventLevel.Verbose);
        }
    }
}