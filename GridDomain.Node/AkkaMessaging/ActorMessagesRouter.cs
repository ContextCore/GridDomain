using Akka.Actor;
using Akka.DI.Core;
using GridDomain.CQRS.Messaging.MessageRouting;
using GridDomain.CQRS.Messaging.MessageRouting.InMemoryRouting;
using NLog;

namespace GridDomain.Node.AkkaMessaging
{
    public class ActorMessagesRouter : IMessagesRouter
    {
        private readonly ActorHandler<CreateRoute> _routingActor;

        public ActorMessagesRouter(IActorRef routingActor)
        {
            _routingActor = new ActorHandler<CreateRoute>(routingActor);
        }

        public IRouteBuilder<TMessage> Route<TMessage>()
        {
            return new AkkaRouteBuilder<TMessage>(_routingActor);
        }
    }
}