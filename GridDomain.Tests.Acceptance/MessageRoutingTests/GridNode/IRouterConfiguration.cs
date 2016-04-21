using GridDomain.Node.AkkaMessaging;

namespace GridDomain.Tests.Acceptance.MessageRoutingTests
{
    public interface IRouterConfiguration
    {
        void ConfigureRouting(ActorMessagesRouter router);
    }
}