using GridDomain.Node.Actors;
using GridDomain.Node.AkkaMessaging.Routing;

namespace GridDomain.Tests.Acceptance.MessageRoutingTests.GridNode
{
    public interface IRouterConfiguration
    {
        void ConfigureRouting(ActorMessagesRouter router);
    }
}