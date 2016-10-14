using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using Akka.Actor;
using GridDomain.CQRS;
using GridDomain.CQRS.Messaging.Akka;

namespace GridDomain.Node.AkkaMessaging.Waiting
{

    public class WaitEndInfo : EventArgs
    {
        public WaitEndInfo(IWaitResults results, object lastMessage)
        {
            Results = results;
            LastMessage = lastMessage;
        }

        public IWaitResults Results { get; }
        public object LastMessage { get; }
    }

    class AkkaMessageLocalWaiter : IDisposable
    {
        private readonly IActorSubscriber _subscriber;
        private readonly TimeSpan _defaultTimeout;
        private readonly ConcurrentBag<MessageInfo> _allMessages = new ConcurrentBag<MessageInfo>();
        private readonly ConcurrentBag<MessageInfo> _allExpectedMessages = new ConcurrentBag<MessageInfo>();

        class MessageInfo
        {
            public readonly Type Type;
            public readonly object Message;

            public MessageInfo(Type type, object message)
            {
                Type = type;
                Message = message;
            }
        }

        private readonly IDictionary<Type, Predicate<object>> _filters = new Dictionary<Type, Predicate<object>>();
        private Predicate<IEnumerable<MessageInfo>> WaitIsOver = c => true;

        private readonly TaskCompletionSource<WaitEndInfo> WaitEnd = new TaskCompletionSource<WaitEndInfo>();
        private readonly Inbox _inbox;

        public AkkaMessageLocalWaiter(ActorSystem system, IActorSubscriber subscriber, TimeSpan defaultTimeout)
        {
            _defaultTimeout = defaultTimeout;
            _subscriber = subscriber;
            _inbox = Inbox.Create(system);
        }

        private void Subscribe<TMsg>(Func<Predicate<IEnumerable<MessageInfo>>, Predicate<IEnumerable<MessageInfo>>>  waitOverConditionMutator,
                                     Predicate<TMsg> filter)
        {
            _filters[typeof(TMsg)] = o => filter((TMsg)o);
            WaitIsOver = waitOverConditionMutator(WaitIsOver);
            _subscriber.Subscribe<TMsg>(_inbox.Receiver);
        }
        public void And<TMsg>(Predicate<TMsg> filter = null)
        {
            filter = filter ?? (t => true);
            Subscribe(oldPredicate => (c => oldPredicate(c) && WasReceived(filter)),filter);
        }

        private bool WasReceived<TMsg>(Predicate<TMsg> filter)
        {
            return _allMessages.OfType<TMsg>().Any(m => filter(m));
        }

        public void Or<TMsg>(Predicate<TMsg> filter = null)
        {
            filter = filter ?? (t => true);
            Subscribe(oldPredicate => (c => oldPredicate(c) || WasReceived(filter)), filter);
        }

        //private bool IsFault(object lastMessage)
        //{
        //    if (!(lastMessage is Failure) && !(lastMessage is IFault) && !(lastMessage is Status.Failure)) return false;
        //    _allMessages.Add(lastMessage);
        //    return true;
        //}

        public void Start(TimeSpan timeout)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            ReceiveWithin(timeout, stopwatch)
                    .ContinueWith(t =>
                    {
                        stopwatch.Stop();
                        var waitResults = new WaitEndInfo(new WaitResults(_allMessages), t.Result);

                        WaitEnd.SetResult(waitResults);
                        WaitComplete(this, waitResults);
                    });
        }

        private Task<object> ReceiveWithin(TimeSpan maxTimeout, Stopwatch watch)
        {
           return _inbox.ReceiveAsync(maxTimeout - watch.Elapsed)
                        .ContinueWith(t =>
                        {
                            var currentMessage = t.Result;
                            _allMessages.Add(new MessageInfo(currentMessage.GetType(), currentMessage));

                            OnMessageReceived(this, currentMessage);

                            return !WaitIsOver(_allExpectedMessages) ? ReceiveWithin(maxTimeout,watch) : currentMessage;

                        }, TaskContinuationOptions.OnlyOnRanToCompletion);
        }

        public event EventHandler<object> OnMessageReceived = delegate { };
        public event EventHandler<object> WaitComplete = delegate { };


        public void Dispose()
        {
            _inbox.Dispose();
        }
    }
}