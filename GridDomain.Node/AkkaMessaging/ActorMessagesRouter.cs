using Akka.Actor;
using GridDomain.CQRS.Messaging.MessageRouting;
using GridDomain.CQRS.Messaging.MessageRouting.InMemoryRouting;
using NLog;

namespace GridDomain.Node.AkkaMessaging
{
    // public class 
    public class ActorMessagesRouter : IMessagesRouter
    {
        private readonly Logger _log = LogManager.GetCurrentClassLogger();
        private readonly ActorHandler<CreateRoute> _routingActor;
        private readonly IActorRef _waiter;

        public ActorMessagesRouter(ActorSystem system,IActorRef waiter)
        {
            _waiter = waiter;
            _routingActor = new ActorHandler<CreateRoute>(system.ActorOf<AkkaRoutingActor>());
        }

        public IRouteBuilder<TMessage> Route<TMessage>()
        {
           return new AkkaRouteBuilder<TMessage>(_routingActor);
        }
    }
}