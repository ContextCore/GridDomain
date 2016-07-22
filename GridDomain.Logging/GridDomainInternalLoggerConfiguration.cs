using Serilog;
using Serilog.Events;

namespace GridDomain.Logging
{
    public class GridDomainInternalLoggerConfiguration : LoggerConfiguration
    {
        public GridDomainInternalLoggerConfiguration()
        {
            WriteTo.RollingFile(".\\GridDomainLogs\\logs-{Date}.txt").
                WriteTo.Elasticsearch("http://soloinfra.cloudapp.net:9222").
                WriteTo.Console(LogEventLevel.Error)
                .Enrich
                .WithMachineName();
        }
    }
}