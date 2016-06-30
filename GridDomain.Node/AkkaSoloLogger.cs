using System;
using Akka.Actor;
using Akka.Event;
using GridDomain.Logging;

namespace GridDomain.Node
{
    public class AkkaSoloLogger : ReceiveActor
    {
        private readonly ISoloLogger _log = LogManager.GetLogger();
        /// <summary>
        /// Initializes a new instance of the <see cref="AkkaSoloLogger"/> class.
        /// </summary>
        public AkkaSoloLogger()
        {
            Receive<Error>(m => WithSerilog(logger => SetContextFromLogEvent(logger, m).Error(m.Cause, GetFormat(m.Message), GetArgs(m.Message))));
            Receive<Warning>(m => WithSerilog(logger => SetContextFromLogEvent(logger, m).Warn(GetFormat(m.Message), GetArgs(m.Message))));
            Receive<Info>(m => WithSerilog(logger => SetContextFromLogEvent(logger, m).Info(GetFormat(m.Message), GetArgs(m.Message))));
            Receive<Debug>(m => WithSerilog(logger => SetContextFromLogEvent(logger, m).Debug(GetFormat(m.Message), GetArgs(m.Message))));
            Receive<InitializeLogger>(m =>
            {
                _log.Info("SerilogLogger started");
                Sender.Tell(new LoggerInitialized());
            });
        }

        private void WithSerilog(Action<ISoloLogger> logStatement)
        {
            logStatement(_log.ForContext("SourceContext", Context.Sender.Path));
        }


        private ISoloLogger SetContextFromLogEvent(ISoloLogger logger, LogEvent logEvent)
        {
            //RequestLogInfoManager.AmbientContext["Timestamp"] = logEvent.Timestamp;
            //RequestLogInfoManager.AmbientContext["LogSource"] = logEvent.LogSource;
            //RequestLogInfoManager.AmbientContext["Thread"] = logEvent.Thread.ManagedThreadId.ToString().PadLeft(4, '0');
            return logger;
        }

        private static string GetFormat(object message)
        {
            var logMessage = message as LogMessage;

            return logMessage != null
                ? logMessage.Format
                : "{Message}";
        }

        private static object[] GetArgs(object message)
        {
            var logMessage = message as LogMessage;

            return logMessage != null
                ? logMessage.Args
                : new[] { message };
        }

    }
}