using System;
using Akka.Actor;
using GridDomain.CQRS.Messaging;

namespace GridDomain.Node
{

    public interface IActorTransport : IPublisher, IActorSubscriber
    {
        
    }

    public interface IActorSubscriber
    {
        void Subscribe<TMessage>(IActorRef actor);
        void Subscribe(Type messageType, IActorRef actor);
    }
}