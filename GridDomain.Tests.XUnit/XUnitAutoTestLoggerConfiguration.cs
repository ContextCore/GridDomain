using GridDomain.Tests.Framework;
using NMoneys;
using Serilog;
using Serilog.Events;
using Xunit.Abstractions;

namespace GridDomain.Tests.XUnit
{
    public class XUnitAutoTestLoggerConfiguration : LoggerConfiguration
    {
        public XUnitAutoTestLoggerConfiguration(ITestOutputHelper output, LogEventLevel level = LogEventLevel.Verbose)
        {
            WriteTo.XunitTestOutput(output);
            MinimumLevel.Is(level);
            Destructure.ByTransforming<Money>(r => new {r.Amount,r.CurrencyCode });
        }
    }
}