using Akka.Cluster.PubSub.Serializers.Proto;
using GridDomain.CQRS.Messaging;
using GridDomain.Node.AkkaMessaging;

namespace GridDomain.Tests.Acceptance.MessageRoutingTests
{
    public abstract class IInitialCommands<T>
    {
        public abstract T[] GetCommands();
    }
}