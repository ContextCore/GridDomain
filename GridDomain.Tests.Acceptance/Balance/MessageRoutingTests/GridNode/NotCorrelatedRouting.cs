using GridDomain.CQRS;
using GridDomain.Node.AkkaMessaging;

namespace GridDomain.Tests.Acceptance.Balance.MessageRoutingTests.GridNode
{
    class NotCorrelatedRouting<TMsg, THandler> : IRouterConfiguration where THandler : IHandler<TMsg>
    { 
  
        public void ConfigureRouting(ActorMessagesRouter router)
        {
            router.Route<TMsg>()
                .ToHandler<THandler>()
                .Register();

            router.WaitForRouteConfiguration();
        }  
    }
}