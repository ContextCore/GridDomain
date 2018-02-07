using Akka.Actor;
using GridDomain.Common;

namespace GridDomain.Tests.Unit.AggregateLifetime.GracefulShutdown {
    public static class AggregateActorDebugExtensions
    {
        public static void TellWithMetadata(this IActorRef actor, object message, IMessageMetadata metadata = null, IActorRef sender=null)
        {
            actor.Tell(new MessageMetadataEnvelop(message,metadata),sender);
        }
    }
}