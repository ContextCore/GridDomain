using GridDomain.CQRS;
using GridDomain.Node.Actors;
using GridDomain.Node.AkkaMessaging.Routing;

namespace GridDomain.Tests.Acceptance.MessageRoutingTests.GridNode
{
    internal class CorrelatedRouting<TMsg, THandler> : IRouterConfiguration
        where THandler : IHandler<TMsg>
    {
        private readonly string _correlationPropertyName;

        public CorrelatedRouting(string correlationPropertyName)
        {
            _correlationPropertyName = correlationPropertyName;
        }

        public void ConfigureRouting(ActorMessagesRouter router)
        {
            router.Route<TMsg>()
                  .ToHandler<THandler>()
                  .WithCorrelation(_correlationPropertyName)
                .Register();
        }
    }
}