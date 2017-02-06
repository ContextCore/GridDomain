using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.Dispatch;
using Akka.Event;
using Serilog;

namespace GridDomain.Node.Actors
{
    public class SerilogLogger : ReceiveActor, IRequiresMessageQueue<ILoggerMessageQueueSemantics>
    {
        private readonly ILogger _logger;

        private static string GetFormat(object message)
        {
            var logMessage = message as LogMessage;
            return logMessage != null ? logMessage.Format : "{Message}";
        }
        private static object[] GetArgs(object message)
        {
            var logMessage = message as LogMessage;
            return logMessage != null ? logMessage.Args : new[] { message };
        }

        private ILogger GetLogger(LogEvent logEvent)
        {
            return _logger.ForContext("SourceContext", Context.Sender.Path)
                          .ForContext("Timestamp", logEvent.Timestamp)
                          .ForContext("LogSource", logEvent.LogSource)
                          .ForContext("Thread", logEvent.Thread.ManagedThreadId.ToString().PadLeft(4, '0'));
        }

        private void Handle(Error logEvent)
        {
            GetLogger(logEvent).Error(logEvent.Cause, GetFormat(logEvent.Message), GetArgs(logEvent.Message));
        }

        private void Handle(Warning logEvent)
        {
            GetLogger(logEvent).Warning(GetFormat(logEvent.Message), GetArgs(logEvent.Message));
        }

        private void Handle(Info logEvent)
        {
            GetLogger(logEvent).Information(GetFormat(logEvent.Message), GetArgs(logEvent.Message));
        }

        private void Handle(Debug logEvent)
        {
            GetLogger(logEvent).Debug(GetFormat(logEvent.Message), GetArgs(logEvent.Message));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SerilogLogger"/> class.
        /// </summary>
        public SerilogLogger()
        {
          //  _logger = logger;

            Receive<Error>(m => Handle(m));
            Receive<Warning>(m => Handle(m));
            Receive<Info>(m => Handle(m));
            Receive<Debug>(m => Handle(m));
            Receive<InitializeLogger>(m =>
            {
                Context.GetLogger().Info("SerilogLogger started");
                Sender.Tell(new LoggerInitialized());
            });
        }
    }
}
