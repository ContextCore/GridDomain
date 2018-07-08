using System;
using Akka.Actor;
using Akka.Event;
using GridDomain.Common;
using Serilog;

namespace GridDomain.Node.Logging {
    public class LoggingExtensionProvider : ExtensionIdProvider<LoggingExtension>
    {
        private readonly ILogger _logger;
        private readonly string _logActorName;
        private readonly TimeSpan _actorCreateTimeout;

        public LoggingExtensionProvider(ILogger logger, string logActorName = null, TimeSpan? actorCreateTimeout = null)
        {
            _actorCreateTimeout = actorCreateTimeout ?? TimeSpan.FromSeconds(5);
            _logActorName = logActorName;

            _logger = logger;
        }

        public override LoggingExtension CreateExtension(ExtendedActorSystem system)
        {
            var logActor = system.SystemActorOf(Props.Create(() => new SerilogLoggerActor(_logger)), _logActorName ?? "node-log-test");
            logActor.Ask<LoggerInitialized>(new InitializeLogger(system.EventStream))
                    .TimeoutAfter(_actorCreateTimeout)
                    .Wait();
            
            return new LoggingExtension(logActor);
        }
    }
}