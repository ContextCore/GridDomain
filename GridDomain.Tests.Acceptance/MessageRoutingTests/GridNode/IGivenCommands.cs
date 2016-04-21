using Akka.Cluster.PubSub.Serializers.Proto;
using GridDomain.CQRS.Messaging;

namespace GridDomain.Tests.Acceptance.MessageRoutingTests
{
    public interface IGivenCommands<T>
    {
        T[] GetCommands();
    }
}