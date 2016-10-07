using System;
using Akka.Actor;
using GridDomain.CQRS;
using GridDomain.CQRS.Messaging.Akka;

namespace GridDomain.Node.AkkaMessaging.Waiting
{
    class AkkaMessageWaiterBuilder : IMessagesWaiterBuilder<IMessageWaiter>
    {
        private readonly AkkaMessageWaiter _waiter;
        public AkkaMessageWaiterBuilder(ActorSystem system, IActorSubscriber subscriber, TimeSpan defaultTimeout)
        {
            _waiter = new AkkaMessageWaiter(system,subscriber,defaultTimeout);
        }

        public IMessageWaiter Create()
        {
            return _waiter;
        }

        public IMessagesWaiterBuilder<IMessageWaiter> Fault<T>(Predicate<IFault<T>> filter = default(Predicate<IFault<T>>))
        {
            _waiter.Subscribe(filter);
            return this;
        }

        public IMessagesWaiterBuilder<IMessageWaiter> Message<T>(Predicate<T> filter = default(Predicate<T>))
        {
            _waiter.Subscribe(filter);
            return this;
        }
    }
}