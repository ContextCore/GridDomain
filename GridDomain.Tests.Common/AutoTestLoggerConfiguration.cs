using NMoneys;
using Serilog;
using Serilog.Events;

namespace GridDomain.Tests.Common
{
    public class AutoTestLoggerConfiguration : LoggerConfiguration
    {
        public AutoTestLoggerConfiguration(bool filterAkkaMessages = true)
        {
            WriteTo.RollingFile("P:\\GridDomain\\Logs\\logs-{yyyy-MM-dd_HH_mm_ss}}.txt");
            WriteTo.Console();
            WriteTo.NUnitOutput();
            MinimumLevel.Is(LogEventLevel.Verbose);
            Destructure.ByTransforming<Money>(r => new {r.Amount, Currency = r.CurrencyCode});

            //  Filter.ByExcluding(e => e.MessageTemplate.Text.Contains(""))
        }
    }
}