using GridDomain.CQRS;
using GridDomain.Node.AkkaMessaging;

namespace GridDomain.Tests.Acceptance.MessageRoutingTests
{
    class CorrelatedRouting<TMsg,THandler> : IRouterConfiguration
        where THandler:IHandler<TMsg>
    {
        private readonly string _correlationPropertyName;
        public CorrelatedRouting(string correlationPropertyName)
        {
            _correlationPropertyName = correlationPropertyName;
        }

        public void ConfigureRouting(ActorMessagesRouter router)
        {
            router.Route<TMsg>()
                .To<THandler>()
                .WithCorrelation(_correlationPropertyName)
                .Register();

            router.WaitForRouteConfiguration();
        }
    }
}