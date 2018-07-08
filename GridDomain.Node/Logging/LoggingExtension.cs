using Akka.Actor;

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
}