using GridDomain.Tests.Framework;
using NMoneys;
using Serilog;
using Serilog.Events;
using Xunit.Abstractions;

namespace GridDomain.Tests.XUnit
{
    public class XUnitAutoTestLoggerConfiguration : LoggerConfiguration
    {
        public XUnitAutoTestLoggerConfiguration(ITestOutputHelper output)
        {
            WriteTo.XunitTestOutput(output);
            MinimumLevel.Is(LogEventLevel.Verbose);
            Destructure.ByTransforming<Money>(r => new {r.Amount,r.CurrencyCode });
        }
    }
}