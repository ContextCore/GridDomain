using Akka.Actor;
using Serilog;

namespace GridDomain.Node.Logging
{
    public class LoggingExtension : IExtension
    {
        public ILogger Logger { get; }
        public IActorRef LogActor { get; }

        public LoggingExtension(IActorRef logActor, ILogger log)
        {
            Logger = log;
            LogActor = logActor;
        }
    }
}