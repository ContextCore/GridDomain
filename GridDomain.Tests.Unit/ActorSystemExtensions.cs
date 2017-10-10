using Akka.Actor;
using Akka.Event;
using GridDomain.Node.Actors.Logging;
using Serilog;

namespace GridDomain.Tests.Unit {
    public static class ActorSystemExtensions
    {
        public static void AttachSerilogLogging(this ActorSystem system, ILogger log , string name = null)
        {
            ExtendedActorSystem actorSystem = (ExtendedActorSystem)system;
            var logActor = actorSystem.SystemActorOf(Props.Create(() => new SerilogLoggerActor(log)), name ?? "node-log-test");
            logActor.Ask<LoggerInitialized>(new InitializeLogger(actorSystem.EventStream)).Wait();
        }
    }
}