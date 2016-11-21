using Akka.Remote.Transport;
using GridDomain.Logging;
using Serilog;

namespace GridDomain.Tests.Framework
{
    public class AutoTestLogFactory : LoggerFactory
    {
        public override ISoloLogger GetLogger(string className = null)
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