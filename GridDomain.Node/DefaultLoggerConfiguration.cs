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
    public static class LoggerConfigurationExtensions
    {
        public const string DefaultTemplate = "{Timestamp:yy-MM-dd HH:mm:ss.fff} [{Level:u3} TH{Thread}] Path:{Path} {NewLine} Src:{SourceContext}"
                                              + "{NewLine} {Message} {NewLine} {Exception} {NewLine}";

        public const string DetailedTemplate = "{Timestamp:yy-MM-dd HH:mm:ss.fff} [{Level:u3} TH{Thread}] Src:{SourceContext}{NewLine}"
                                              + "Class:{ClassName}{NewLine}"
                                              + "{Message}{NewLine}"
                                              + "{Exception}";

        public static string GetTemplate(this LogEventLevel level)
        {
           // return  level >= LogEventLevel.Information ? DefaultTemplate : DetailedTemplate;
            return DetailedTemplate;
        }

        public static LoggerConfiguration WriteToFile(this LoggerConfiguration cfg, LogEventLevel level, string fileName = null)
        {
            var template =  level.GetTemplate();
            if (fileName != null)
            {
                return cfg.WriteTo.File($"./Logs/{fileName}.log", level, template);
            }
            else
                return cfg.WriteTo.RollingFile("./Logs/log_{HalfHour}.txt", level, template);
        }

        public static LoggerConfiguration Default(this LoggerConfiguration cfg, LogEventLevel level)
        {
            return cfg.Enrich.FromLogContext()
                      .MinimumLevel.Is(level)
                      .Destructure.ByTransforming<Money>(r => new {r.Amount, r.CurrencyCode})
                      .Destructure.ByTransforming<Exception>(r => new {Type = r.GetType(), r.Message, r.StackTrace})
                      .Destructure.ByTransforming<MessageMetadata>(r => new {r.CasuationId, r.CorrelationId})
                      .Destructure.ByTransforming<IMessageMetadataEnvelop>(r => new {r.Message, r.Metadata})
                      .Destructure.ByTransforming<PersistEventPack>(r => new {Size = r.Events?.Length})
                      .Destructure.ByTransforming<MessageMetadataEnvelop<ICommand>>(r => new {CommandType = r.Message.GetType(), CommandId = r.Message.Id, r.Metadata})
                      .Destructure.ByTransforming<MessageMetadataEnvelop<DomainEvent>>(r => new {EventType = r.Message.GetType(), EventId = r.Message.Id, r.Metadata})
                      .Destructure.ByTransforming<AggregateCommandExecutionContext>(r => new {CommandId = r.Command?.Id, Metadata = r.CommandMetadata})
                      .Destructure.ByTransforming<ProcessesTransitComplete>(r => new {Event = r.InitialMessage, ProducedCommandsNum = r.ProducedCommands.Count})
                      .Destructure.ByTransforming<CreateNewProcess>(r => new {Event = (r.Message?.Message as IHaveId)?.Id ?? r.Message?.Message, r.EnforcedId, r.Message?.Metadata});
        }
    }

    public class DefaultLoggerConfiguration : LoggerConfiguration
    {
        public DefaultLoggerConfiguration(LogEventLevel level = LogEventLevel.Verbose, string fileName = null)
        {
            this.Default(level);
            this.WriteToFile(level, fileName);
        }
    }
}