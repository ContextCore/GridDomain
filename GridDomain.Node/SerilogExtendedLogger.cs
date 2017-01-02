using System;
using Akka.Actor;
using Akka.Event;
using GridDomain.Logging;
using Debug = Akka.Event.Debug;

namespace GridDomain.Node
{
    public class SerilogExtendedLogger : ReceiveActor
    {
        private readonly ILogger _log = LogManager.GetLogger();
        /// <summary>
        /// Initializes a new instance of the <see cref="SerilogExtendedLogger"/> class.
        /// </summary>
        public SerilogExtendedLogger()
        {
          
            Receive<Error>(m => WithSerilog(logger => SetContextFromLogEvent(logger, m).Error(m.Cause, GetFormat(m.Message), GetArgs(m.Message))));
            Receive<Warning>(m => WithSerilog(logger => SetContextFromLogEvent(logger, m).Warn(GetFormat(m.Message), GetArgs(m.Message))));
            Receive<Info>(m => WithSerilog(logger => SetContextFromLogEvent(logger, m).Info(GetFormat(m.Message), GetArgs(m.Message))));
            Receive<Debug>(m => WithSerilog(logger => SetContextFromLogEvent(logger, m).Debug(GetFormat(m.Message), GetArgs(m.Message))));
            Receive<InitializeLogger>(m =>
            {
                _log.Debug("SerilogLogger started");
                Sender.Tell(new LoggerInitialized());
            });
        }

        private void WithSerilog(Action<ILogger> logStatement)
        {
            logStatement(_log.ForContext("SourceContext", Context.Sender.Path));
        }


        private ILogger SetContextFromLogEvent(ILogger logger, LogEvent logEvent)
        {
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