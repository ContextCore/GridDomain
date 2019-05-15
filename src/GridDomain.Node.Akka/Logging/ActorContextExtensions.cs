using Akka.Actor;
using Akka.Event;
//using Akka.Logger.Serilog;

namespace GridDomain.Node.Akka.Logging
{

    public static class ActorContextExtensions
    {
        public static ILoggingAdapter GetSeriLogger(this IUntypedActorContext context)
        {
            return context.GetLogger();//GetLogger<SerilogLoggingAdapter>();
        }
    }
}