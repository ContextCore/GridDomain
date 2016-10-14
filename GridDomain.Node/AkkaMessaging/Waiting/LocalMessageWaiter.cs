using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Threading.Tasks;
using Akka;
using Akka.Actor;
using GridDomain.Common;
using GridDomain.CQRS;
using GridDomain.Logging;
using GridDomain.Node.AkkaMessaging.Waiting;

namespace GridDomain.Node.AkkaMessaging.Waiting
{
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