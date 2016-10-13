using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Akka.Actor;
using GridDomain.CQRS;
using GridDomain.CQRS.Messaging.Akka;

namespace GridDomain.Node.AkkaMessaging.Waiting
{
    class MessageWaiterConfigurator<TWaiter> : IMessagesWaiterBuilder<TWaiter> where TWaiter : LocalMessageWaiter
    {
        private readonly TWaiter _waiter;
        private readonly List<Task<object>> _allTasks = new List<Task<object>>();
        private readonly IActorSubscriber _subscriber;

        public MessageWaiterConfigurator(Func<TWaiter> waiterFactory, IActorSubscriber subscriber)
        {
            _subscriber = subscriber;
            _waiter = waiterFactory();
        }

        public IMessagesWaiterBuilder<TWaiter> Message<T>(Predicate<T> filter = null)
        {
            _subscriber.Subscribe<T>(_waiter.Receiver);
            _allTasks.Add(_waiter.Receive(filter).ContinueWith(t => (object)t.Result));
            return this;
        }

        public IMessagesWaiterBuilder<TWaiter> Fault<T>(Predicate<IFault<T>> filter = null)
        {
            _subscriber.Subscribe<IFault<T>>(_waiter.Receiver);
            _allTasks.Add(_waiter.Receive(filter).ContinueWith(t => (object)t.Result));
            return this;
        }

        public TWaiter Create()
        {
            return _waiter;
        }
    }
}