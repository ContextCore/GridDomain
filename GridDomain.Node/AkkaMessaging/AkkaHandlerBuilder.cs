using System.Threading.Tasks;
using Akka.Actor;
using GridDomain.Common;
using GridDomain.CQRS;
using GridDomain.CQRS.Messaging.MessageRouting;
using GridDomain.Node.Actors;
using GridDomain.Node.AkkaMessaging.Routing;

namespace GridDomain.Node.AkkaMessaging
{
    public class AkkaHandlerBuilder<TMessage, THandler> :
        IHandlerBuilder<TMessage, THandler> where THandler : IHandler<TMessage>
    {
        private readonly IActorRef _routingRegistrator;
        public string CorrelationPropertyName;

        public AkkaHandlerBuilder(IActorRef routingRegistrator)
        {
            _routingRegistrator = routingRegistrator;
        }

        public Task Register()
        {
            var routeMessage = new CreateHandlerRouteMessage(typeof(IMessageMetadataEnvelop<TMessage>),
                                                             typeof(THandler),
                                                             CorrelationPropertyName,
                                                             PoolKind.ConsistentHash);

            return _routingRegistrator.Ask<RouteCreated>(routeMessage);
        }

        public IHandlerBuilder<TMessage, THandler> WithCorrelation(string name)
        {
            CorrelationPropertyName = name;
            return this;
        }
    }
}