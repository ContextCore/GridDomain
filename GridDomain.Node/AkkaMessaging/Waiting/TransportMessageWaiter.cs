using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Akka.Actor;
using GridDomain.CQRS.Messaging.Akka;

namespace GridDomain.Node.AkkaMessaging.Waiting
{
    internal class TransportMessageWaiter : IMessageWaiter
    {
        public IActorRef Receiver { get; }
        private readonly TimeSpan _timeout;
        private IActorSubscriber _subscriber;
        private ConcurrentBag<object> _receivedMessages = new ConcurrentBag<object>();

        protected TransportMessageWaiter(ActorSystem system, IActorSubscriber subscriber)
        {
            _subscriber = subscriber;
            var inbox = Inbox.Create(system);
            inbox.ReceiveAsync();
        }

        private void SubscribeToInbox(Inbox inbox)
        {
            inbox.ReceiveAsync().ContinueWith(t => 
            {
                _receivedMessages.Add();
            })   
        }

        public Task<IWaitResults> ReceiveAll()
        {
            //return Receiver.Ask<ExpectedMessagesReceived>(NotifyOnWaitEnd.Instance, _timeout)
            //               .ContinueWith(t => (IWaitResults)new WaitResults(t.Result.Received),
            //                             TaskContinuationOptions.OnlyOnRanToCompletion);
            throw new NotImplementedException();
        }

        public Task<TMsg> Receive<TMsg>(Predicate<TMsg> filter = null)
        {
            return Receiver.Ask<TMsg>(new NotifyOnMessage(typeof(TMsg)), _timeout)
                .ContinueWith(t => GetMessage(filter, t),
                    TaskContinuationOptions.OnlyOnRanToCompletion);
        }

        private TMsg GetMessage<TMsg>(Predicate<TMsg> filter, Task<TMsg> t)
        {
            return filter?.Invoke(t.Result) == true ? t.Result : Receive(filter).Result;
        }
    }
}