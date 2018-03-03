using System;
using System.IO;
using System.Linq;
using Akka.Event;
using GridDomain.Common;
using GridDomain.CQRS;
using GridDomain.EventSourcing;
using GridDomain.Node.Actors.Aggregates;
using GridDomain.Node.Actors.Aggregates.Messages;
using GridDomain.Node.Actors.CommandPipe.Messages;
using GridDomain.Node.Actors.ProcessManagers;
using GridDomain.Node.Actors.ProcessManagers.Messages;
using NMoneys;
using Serilog;
using Serilog.Core;
using Serilog.Events;

namespace GridDomain.Node
{
    public class DefaultLoggerConfiguration : LoggerConfiguration
    {
        public const string DefaultTemplate = "{Timestamp:yy-MM-dd HH:mm:ss.fff} [{Level:u3} TH{Thread}] Src:{LogSource}"
                                                 + "{NewLine} {Message} {NewLine} {Exception} {NewLine}";

        public DefaultLoggerConfiguration(LogEventLevel level = LogEventLevel.Verbose, string fileName = null)
        {
            Enrich.FromLogContext();
            if (fileName != null)
            {
                if(File.Exists(fileName))
                    File.Delete(fileName);
                WriteTo.File(fileName, level, DefaultTemplate);
            }
            else 
                WriteTo.RollingFile(".\\Logs\\log_{HalfHour}.txt", level,DefaultTemplate);
            
            MinimumLevel.Is(level);
            Destructure.ByTransforming<Money>(r => new { r.Amount, r.CurrencyCode });
            Destructure.ByTransforming<Exception>(r => new { Type = r.GetType(), r.StackTrace });
            Destructure.ByTransforming<MessageMetadata>(r => new { r.CasuationId, r.CorrelationId });
            Destructure.ByTransforming<PersistEventPack>(r => new { Size = r.Events?.Length });
            Destructure.ByTransforming<MessageMetadataEnvelop<ICommand>>(r => new { CommandId = r.Message.Id, r.Metadata });
            Destructure.ByTransforming<MessageMetadataEnvelop<DomainEvent>>(r => new { EventId = r.Message.Id, r.Metadata });
            Destructure.ByTransforming<AggregateCommandExecutionContext>(r => new { CommandId = r.Command?.Id, Metadata = r.CommandMetadata });
            Destructure.ByTransforming<ProcessesTransitComplete>(r => new { Event = r.InitialMessage, ProducedCommandsNum = r.ProducedCommands.Length });
            Destructure.ByTransforming<CreateNewProcess>(r => new { Event = (r.Message?.Message as IHaveId)?.Id ?? r.Message?.Message, r.EnforcedId, r.Message?.Metadata });
        }
    }

}