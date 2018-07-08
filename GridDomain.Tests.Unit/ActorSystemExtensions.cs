using Akka.Actor;
using Akka.Event;
using GridDomain.Node.Actors.Logging;
using GridDomain.Node.Logging;
using GridDomain.Tools;
using Serilog;

namespace GridDomain.Tests.Unit {
    public static class ActorSystemExtensions
    {
        public static void AttachSerilogLogging(this ActorSystem system, ILogger log , string name = null)
        {
            ExtendedActorSystem actorSystem = (ExtendedActorSystem)system;

            if (actorSystem.GetExtension<LoggingExtension>() == null)
                actorSystem.InitSerilogExtension(log, name);
        }
    }
}