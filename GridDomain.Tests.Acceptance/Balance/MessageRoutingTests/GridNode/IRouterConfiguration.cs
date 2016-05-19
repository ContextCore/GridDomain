using GridDomain.Node.AkkaMessaging;

namespace GridDomain.Tests.Acceptance.MessageRoutingTests.GridNode
{
    public interface IRouterConfiguration
    {
        void ConfigureRouting(ActorMessagesRouter router);
    }
}