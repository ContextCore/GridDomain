using System;
using Akka.Event;
using NMoneys;
using Serilog;
using Serilog.Events;

namespace GridDomain.Node
{
    public class DefaultLoggerConfiguration : LoggerConfiguration
    {
        public DefaultLoggerConfiguration()
        {
            WriteTo.RollingFile(".\\GridDomainLogs\\logs-{yyyy-MM-dd_HH_mm_ss}}.txt");
            WriteTo.Console();
            MinimumLevel.Is(LogEventLevel.Verbose);
            Destructure.ByTransforming<Money>(r => new {r.Amount, Currency = r.CurrencyCode});
        }
    }
    
}