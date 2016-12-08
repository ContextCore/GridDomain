using System.Threading.Tasks;
using Akka.Actor;
using GridDomain.CQRS;
using GridDomain.CQRS.Messaging.MessageRouting;
using GridDomain.Node.Actors;

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
            return _routingRegistrator.Ask<RouteCreated>(CreateHandlerRouteMessage.New<TMessage, THandler>(CorrelationPropertyName));
        }

        public IHandlerBuilder<TMessage, THandler> WithCorrelation(string name)
        {
            CorrelationPropertyName = name;
            return this;
        }
    }
}