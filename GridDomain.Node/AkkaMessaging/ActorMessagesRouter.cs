using System;
using System.Threading;
using Akka.Actor;
using Akka.DI.Core;
using GridDomain.CQRS.Messaging.MessageRouting;
using NLog;

namespace GridDomain.Node.AkkaMessaging
{

    public class ActorMessagesRouter : IMessagesRouter
    {
        private readonly TypedMessageActor<CreateRoute> _routingTypedMessageActor;
        private readonly TypedMessageActor<CreateActorRoute> _routingActorTypedMessageActor;
        private readonly IAggregateActorLocator _actorLocator;

        public ActorMessagesRouter(IActorRef routingActor, IAggregateActorLocator actorLocator)
        {
            _actorLocator = actorLocator;
            _routingTypedMessageActor = new TypedMessageActor<CreateRoute>(routingActor);
            _routingActorTypedMessageActor = new TypedMessageActor<CreateActorRoute>(routingActor);
        }

        public IRouteBuilder<TMessage> Route<TMessage>()
        {
            return new AkkaRouteBuilder<TMessage>(_routingTypedMessageActor, _routingActorTypedMessageActor, _actorLocator);
        }
        //TODO:replace with wait until event notifications
        public void WaitForRouteConfiguration()
        {
            Thread.Sleep(TimeSpan.FromSeconds(3));
        }
    }
}