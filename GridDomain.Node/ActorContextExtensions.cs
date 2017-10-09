using Akka.Actor;
using Akka.Event;
using GridDomain.Node.Actors.Logging;

namespace GridDomain.Node
{

    public static class ActorContextExtensions
    {
        public static ILoggingAdapter GetSeriLogger(this IUntypedActorContext context)
        {
            return context.GetLogger(new SerilogLogMessageFormatter());
        }
    }
}