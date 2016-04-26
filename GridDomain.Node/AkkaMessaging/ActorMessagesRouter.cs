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

        public ActorMessagesRouter(IActorRef routingActor)
        {
            _routingTypedMessageActor = new TypedMessageActor<CreateRoute>(routingActor);
        }

        public IRouteBuilder<TMessage> Route<TMessage>()
        {
            return new AkkaRouteBuilder<TMessage>(_routingTypedMessageActor);
        }
        //TODO:replace with wait until event notifications
        public void WaitForRouteConfiguration()
        {
            Thread.Sleep(TimeSpan.FromSeconds(3));
        }
    }
}