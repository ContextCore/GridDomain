using System;
using GridDomain.Common;
using GridDomain.CQRS;
using GridDomain.Node.Actors.Aggregates;
using GridDomain.Node.Actors.Aggregates.Messages;
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
            //Destructure.ByTransforming<IMessageMetadata>(r => new {r.CasuationId, r.CorrelationId});
            Destructure.ByTransforming<MessageMetadata>(r => new {r.CasuationId, r.CorrelationId});
           // Destructure.ByTransforming<Command>(r => new {r.Id});
            Destructure.ByTransforming<MessageMetadataEnvelop<ICommand>>(r => new {Command = r.Message, Metadata = r.Metadata});
            Destructure.ByTransforming<PersistEventPack>(r => new {Size = r.Events.Length});
            Destructure.ByTransforming<AggregateCommandExecutionContext>(r => new {Command = r.Command.Id, Metadata = r.CommandMetadata});
        
        }
    }
}