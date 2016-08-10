using Serilog;
using Serilog.Events;

namespace GridDomain.Tests.Framework
{
    class AutoTestLogConfig: LoggerConfiguration
    {
        public AutoTestLogConfig(LogEventLevel minLogLevel = LogEventLevel.Warning)
        {
            WriteTo.RollingFile(".\\GridDomainLogs\\logs-{yyyy-MM-dd_HH_mm_ss}}.txt")
            .WriteTo.Console(minLogLevel);
        }
    }
}