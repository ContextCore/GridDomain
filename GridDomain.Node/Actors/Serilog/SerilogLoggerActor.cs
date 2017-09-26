using System.Linq;
using Akka.Actor;
using Akka.Dispatch;
using Akka.Event;
using Serilog;
using LogEvent = Akka.Event.LogEvent;

namespace GridDomain.Node.Actors.Serilog
{
   
    public class SerilogLoggerActor : ReceiveActor,
                                      IRequiresMessageQueue<ILoggerMessageQueueSemantics>
    {
        private readonly ILogger _logger;

        public SerilogLoggerActor() : this(Log.Logger)
        {
            
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="SerilogLogger" /> class.
        /// </summary>
        public SerilogLoggerActor(ILogger log)
        {
            _logger = log;

            Receive<Error>(m => Handle(m));
            Receive<Warning>(m => Handle(m));
            Receive<Info>(m => Handle(m));
            Receive<Debug>(m => Handle(m));
            Receive<InitializeLogger>(m =>
                                      {
                                          Context.GetLogger().Info("SerilogLogger started");
                                          m.LoggingBus.Subscribe(Self, typeof(LogEvent));
                                          Sender.Tell(new LoggerInitialized());
                                      });
        }

        private static string GetFormat(object message)
        {
            var logMessage = message as LogMessage;
            var defaultFormat = "{Message}";
            return logMessage == null ? defaultFormat : logMessage.Format;
        }

        private static object[] GetArgs(object message)
        {
            if (message is LogMessage logMessage) return logMessage.Args;
            return new[] {message};
        }

        private ILogger GetLogger(LogEvent logEvent)
        {
            return _logger.ForContext("Timestamp", logEvent.Timestamp)
                          .ForContext("LogSource", "["+logEvent.LogSource.Split('/').Last())
                          .ForContext("Thread", logEvent.Thread.ManagedThreadId);
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
    }
}