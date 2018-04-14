using System;
using Akka.Actor;
using Akka.Event;
using GridDomain.Common;
using Serilog;

namespace GridDomain.Node.Logging
{
    public class LoggingExtension : IExtension
    {
        public IActorRef Logger { get; }

        public LoggingExtension(IActorRef logger)
        {
            Logger = logger;
        }
    }

    public static class LoggingExtensions
    {
        public static LoggingExtension InitSerilogExtension(this ActorSystem system, ILogger log, string name=null)
        {
            return (LoggingExtension) system.RegisterExtension(new LoggingExtensionProvider(log,name));
        }
    }

    public class LoggingExtensionProvider : ExtensionIdProvider<LoggingExtension>
    {
        private readonly ILogger _loggerActor;
        private string _logActorName;
        private readonly TimeSpan _actorCreateTimeout;

        public LoggingExtensionProvider(ILogger loggerActor, string logActorName = null, TimeSpan? actorCreateTimeout = null)
        {
            _actorCreateTimeout = actorCreateTimeout ?? TimeSpan.FromSeconds(5);
            _logActorName = logActorName;

            _loggerActor = loggerActor;
        }

        public override LoggingExtension CreateExtension(ExtendedActorSystem system)
        {
            var logActor = system.SystemActorOf(Props.Create(() => new SerilogLoggerActor(_loggerActor)), _logActorName ?? "node-log-test");
            logActor.Ask<LoggerInitialized>(new InitializeLogger(system.EventStream))
                    .TimeoutAfter(_actorCreateTimeout)
                    .Wait();
            return new LoggingExtension(logActor);
        }
    }
}