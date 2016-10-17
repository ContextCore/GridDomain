using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Akka.Actor;
using GridDomain.CQRS.Messaging.Akka;

namespace GridDomain.Node.AkkaMessaging.Waiting
{
    public class ExpectBuilder
    {
        public Predicate<IEnumerable<object>> WaitCondition = c => true;
        private readonly IActorSubscriber _subscriber;
        private readonly IActorRef _receiver;
        private readonly IReadOnlyCollection<object> _receivedMessages;
        private readonly AkkaMessageLocalWaiter _waiter;

        public ExpectBuilder(AkkaMessageLocalWaiter waiter,
            IActorSubscriber subscriber, 
            IActorRef receiver, 
            IReadOnlyCollection<object> receivedMessages)
        {
            _waiter = waiter;
            _receivedMessages = receivedMessages;
            _receiver = receiver;
            _subscriber = subscriber;
        }

        public Task<IWaitResults> Within(TimeSpan timeout)
        {
            return _waiter.Start(timeout);
        }

 
        public ExpectBuilder And<TMsg>(Predicate<TMsg> filter = null)
        {
            filter = filter ?? (t => true);
            _waiter.Subscribe(oldPredicate => (c => oldPredicate(c) && _waiter.WasReceived(filter)), filter);
            return this;
        }
        public ExpectBuilder Or<TMsg>(Predicate<TMsg> filter = null)
        {
            filter = filter ?? (t => true);
            _waiter.Subscribe(oldPredicate => (c => oldPredicate(c) || _waiter.WasReceived(filter)), filter);
            return this;
        }

   
    }
}