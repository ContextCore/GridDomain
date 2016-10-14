using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Threading.Tasks;
using Akka;
using Akka.Actor;
using GridDomain.Common;
using GridDomain.CQRS;
using GridDomain.CQRS.Messaging.Akka;
using GridDomain.Logging;
using GridDomain.Node.AkkaMessaging.Waiting;

namespace GridDomain.Node.AkkaMessaging.Waiting
{

    class AllMessagesWaiter : LocalMessageWaiter
    {
        public AllMessagesWaiter(ActorSystem system, TimeSpan timeout) 
            : base(CreateWaiter(system),timeout)
        {
        }

        private static IActorRef CreateWaiter(ActorSystem system)
        {
            var props = Props.Create(() => new AllMessageWaiterActor(null, null, null));
            return system.ActorOf(props, "MessagesWaiter_" + Guid.NewGuid());
        }
    }


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
            inbox.ReceiveAsync()
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


    internal class LocalMessageWaiter : IMessageWaiter
    {
        public IActorRef Receiver { get; }
        private readonly TimeSpan _timeout;

        protected LocalMessageWaiter(IActorRef receiver, TimeSpan timeout)
        {
            _timeout = timeout;
            Receiver = receiver;
        }

        public Task<IWaitResults> ReceiveAll()
        {
            return Receiver.Ask<ExpectedMessagesReceived>(NotifyOnWaitEnd.Instance, _timeout)
                           .ContinueWith(t => (IWaitResults)new WaitResults(t.Result.Received), 
                                         TaskContinuationOptions.OnlyOnRanToCompletion);
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