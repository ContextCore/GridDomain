using GridDomain.CQRS;
using GridDomain.Node.AkkaMessaging;

namespace GridDomain.Tests.Acceptance.MessageRoutingTests
{
    class NotCorrelatedRouting<TMsg, THandler> : IRouterConfiguration where THandler : IHandler<TMsg>
    { 
  
        public void ConfigureRouting(ActorMessagesRouter router)
        {
            router.Route<TMsg>()
                .To<THandler>()
                .Register();

            router.WaitForRouteConfiguration();
        }  
    }
}