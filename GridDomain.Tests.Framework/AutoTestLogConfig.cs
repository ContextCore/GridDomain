using Serilog;
using Serilog.Events;

namespace GridDomain.Tests.Framework
{
    class AutoTestLogConfig: LoggerConfiguration
    {

        public AutoTestLogConfig(LogEventLevel minLogLevel = LogEventLevel.Warning)
        {
            WriteTo.RollingFile(".\\GridDomainLogs\\logs-{Date}.txt")
           .WriteTo.ColoredConsole(minLogLevel);
        }
    }
}