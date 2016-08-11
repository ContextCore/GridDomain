using GridDomain.CQRS;
using GridDomain.Node;

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
            router.RegisterHandler<TMsg, THandler>(_correlationPropertyName);
        }
    }
}