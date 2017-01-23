using Helios.Util;
using NMoneys;
using Serilog;
using Serilog.Events;

namespace GridDomain.Tests.Framework
{ 
    public class AutoTestLoggerConfiguration : LoggerConfiguration
    {
        public AutoTestLoggerConfiguration()
        {
            WriteTo.RollingFile("P:\\GridDomain\\Logs\\logs-{yyyy-MM-dd_HH_mm_ss}}.txt");
            WriteTo.Console();
            WriteTo.NUnitOutput();
            MinimumLevel.Is(LogEventLevel.Verbose);
            Destructure.ByTransforming<Money>(r => new { Amount = r.Amount, Currency = r.CurrencyCode });
        }
    }
}