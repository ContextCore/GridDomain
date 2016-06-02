using GridDomain.Node.AkkaMessaging;

namespace GridDomain.Tests.Acceptance.Balance.MessageRoutingTests.GridNode
{
    public interface IRouterConfiguration
    {
        void ConfigureRouting(ActorMessagesRouter router);
    }
}