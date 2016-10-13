using System;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Threading.Tasks;
using Akka;
using Akka.Actor;
using GridDomain.Common;
using GridDomain.CQRS;
using GridDomain.Logging;

namespace GridDomain.Node.AkkaMessaging.Waiting
{
    class LocalMessageWaiter : IMessageWaiter
    {
        private readonly ISoloLogger _logger = LogManager.GetLogger();
        public IActorRef Receiver { get; }
        private readonly TimeSpan _timeout;

        internal LocalMessageWaiter(IActorRef receiver, TimeSpan timeout)
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
                .ContinueWith(t => filter?.Invoke(t.Result) == true ?
                    t.Result : Receive(filter).Result, 
                    TaskContinuationOptions.OnlyOnRanToCompletion);
        }

        public Task<T> ReceiveAll<T>()
        {
            return Receiver.Ask<object>(NotifyOnWaitEnd.Instance, _timeout)
                .ContinueWith(ProcessWaitResults)
                .ContinueWithSafeResultCast(result => (T)result);
        }

        private object ProcessWaitResults(Task<object> t)
        {
            if (t.IsCanceled)
                throw new TimeoutException("Command execution timed out");

            object result = null;
            t.Result.Match()
                .With<ExpectedMessagesReceived>(e =>
                {
                    result = e.Received.Count > 1 ? e.Received.ToArray() : e.Received.First();
                })
                .With<IFault>(fault =>
                {
                    var domainExcpetion = fault.Exception.UnwrapSingle();
                    ExceptionDispatchInfo.Capture(domainExcpetion).Throw();
                })
                .With<Failure>(f =>
                {
                    if (f.Exception is TimeoutException)
                        throw new TimeoutException("Command execution timed out");
                    ThrowInvalidMessage(f);
                })
                .With<Status.Failure>(s =>
                {
                    if (s.Cause is TimeoutException)
                        throw new TimeoutException("Command execution timed out");
                    ThrowInvalidMessage(s);
                })
                .Default(m =>
                {
                    result = m;
                });

            return result;
        }

        private void ThrowInvalidMessage(object m)
        {
            var invalidMessageException = new InvalidMessageException(m.ToPropsString());
            _logger.Error(invalidMessageException, "Received unexpected message while waiting for command execution: {Message}",
                m.ToPropsString());
            throw invalidMessageException;
        }

        public static LocalMessageWaiter New(ActorSystem system, TimeSpan timeout)
        {
            var props = Props.Create(() => new AllMessageWaiterActor(null, null, null));
            var waitActor = system.ActorOf(props, "MessagesWaiter_" + Guid.NewGuid());

            return new LocalMessageWaiter(waitActor, timeout);
        }
    }
}