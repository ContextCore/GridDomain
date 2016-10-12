using System;
using Akka.Actor;
using GridDomain.CQRS.Messaging;
using GridDomain.CQRS.Messaging.Akka;
using GridDomain.Node.AkkaMessaging.Waiting;

namespace GridDomain.Node
{
    public class AkkaMessagesWaiterBuilder : IMessageWaiterProducer
    {
        public AkkaMessagesWaiterBuilder(ActorSystem system, IActorSubscriber subscriber, TimeSpan defaultTimeout)
        {
            
        }
        public IMessagesWaiterBuilder<IMessageWaiter> NewWaiter()
        {
           //return new AkkaMessageWaiterBuilder();
            return null;
        }

        public IMessagesWaiterBuilder<ICommandWaiter> NewCommandWaiter()
        {
            throw new NotImplementedException();
        }
    }
}