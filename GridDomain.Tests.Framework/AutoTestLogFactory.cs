using Akka.Remote.Transport;
using GridDomain.Logging;
using Serilog;
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

        private LoggerConfiguration GetConfiguration()
        {
            return new LoggerConfiguration().WriteTo.RollingFile(".\\GridDomainLogs\\logs-{yyyy-MM-dd_HH_mm_ss}}.txt")
            .WriteTo.Console();
        }
    }
}