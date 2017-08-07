using System;
using GridDomain.Common;
using GridDomain.CQRS;
using GridDomain.Tests.Common;
using NMoneys;
using Serilog;
using Serilog.Events;
using Xunit.Abstractions;

namespace GridDomain.Tests.Unit
{
    public class XUnitAutoTestLoggerConfiguration : LoggerConfiguration
    {
        public XUnitAutoTestLoggerConfiguration(ITestOutputHelper output, LogEventLevel level = LogEventLevel.Verbose)
        {
            WriteTo.XunitTestOutput(output,level,
                  "{Timestamp:yy-MM-dd HH:mm:ss.fff} [{Level:u3} TH{Thread}] Src:{LogSource}"
                + "{NewLine} Message: {Message}"
                + "{NewLine} {Exception}");

            //cannot enrich from context as it is static and logger is interested in instance-specifica data
            Enrich.FromLogContext();
            MinimumLevel.Is(level);

            Destructure.ByTransforming<Money>(r => new {r.Amount, r.CurrencyCode});
            Destructure.ByTransforming<Exception>(r => new {Type = r.GetType(), r.StackTrace});
            Destructure.ByTransforming<IMessageMetadata>(r => new {r.CasuationId, r.CorrelationId});
            Destructure.ByTransforming<ICommand>(r => new {r.Id});
        }
    }
}