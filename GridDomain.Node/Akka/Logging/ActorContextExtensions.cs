using Akka.Actor;
using Akka.Event;
using Akka.Logger.Serilog;

namespace GridDomain.Node
{

    public static class ActorContextExtensions
    {
        public static ILoggingAdapter GetSeriLogger(this IUntypedActorContext context)
        {
            return context.GetLogger<SerilogLoggingAdapter>();
        }
    }
}