using Akka.Actor;
using GridDomain.CQRS.Messaging.MessageRouting;
using GridDomain.CQRS.Messaging.MessageRouting.InMemoryRouting;
using NLog;

namespace GridDomain.Node.AkkaMessaging
{
    public class ActorMessagesRouter : IMessagesRouter
    {
        private readonly ActorHandler<CreateRoute> _routingActor;

        public ActorMessagesRouter(ActorSystem system)
        {
            _routingActor = new ActorHandler<CreateRoute>(system.ActorOf<AkkaRoutingActor>());
        }

        public IRouteBuilder<TMessage> Route<TMessage>()
        {
            return new AkkaRouteBuilder<TMessage>(_routingActor);
        }
    }
}