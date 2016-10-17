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
    public class AkkaMessageLocalWaiter : IDisposable
    {
        private readonly IActorSubscriber _subscriber;
        private readonly ConcurrentBag<object> _allMessages = new ConcurrentBag<object>();
        private readonly ConcurrentBag<object> _allExpectedMessages = new ConcurrentBag<object>();

        public event EventHandler<object> OnMessageReceived = delegate { };
        public event EventHandler<object> WaitComplete = delegate { };

        private readonly IDictionary<Type, Predicate<object>> _filters = new Dictionary<Type, Predicate<object>>();
        private Predicate<IEnumerable<object>> WaitIsOver = c => true;

        private readonly TaskCompletionSource<WaitEndInfo> WaitEnd = new TaskCompletionSource<WaitEndInfo>();
        private readonly Inbox _inbox;
        private ExpectBuilder _expectBuilder;


        public AkkaMessageLocalWaiter(ActorSystem system, IActorSubscriber subscriber)
        {
            _subscriber = subscriber;
            _inbox = Inbox.Create(system);
            _expectBuilder = new ExpectBuilder(this,subscriber,_inbox.Receiver,_allMessages);
        }

        internal void Subscribe<TMsg>(Func<Predicate<IEnumerable<object>>, Predicate<IEnumerable<object>>> waitOverConditionMutator,
                                     Predicate<TMsg> filter)
        {
            _filters[typeof(TMsg)] = o => filter((TMsg)o);
            WaitIsOver = waitOverConditionMutator(WaitIsOver);
            _subscriber.Subscribe<TMsg>(_inbox.Receiver);
        }

        public ExpectBuilder Expect<TMsg>(Predicate<TMsg> filter = null)
        {
            return _expectBuilder.And<TMsg>(filter);
        }
        //public void And<TMsg>(Predicate<TMsg> filter = null)
        //{
        //    filter = filter ?? (t => true);
        //    Subscribe(oldPredicate => (c => oldPredicate(c) && WasReceived(filter)),filter);
        //}
        internal bool WasReceived<TMsg>(Predicate<TMsg> filter)
        {
            return _allMessages.OfType<TMsg>().Any(m => filter(m));
        }


        //public void Or<TMsg>(Predicate<TMsg> filter = null)
        //{
        //    filter = filter ?? (t => true);
        //    Subscribe(oldPredicate => (c => oldPredicate(c) || WasReceived(filter)), filter);
        //}

        //private bool IsFault(object lastMessage)
        //{
        //    if (!(lastMessage is Failure) && !(lastMessage is IFault) && !(lastMessage is Status.Failure)) return false;
        //    _allMessages.Add(lastMessage);
        //    return true;
        //}
        public Task<IWaitResults> WhenReceiveAll()
        {
            return WaitEnd.Task.ContinueWith(t => t.Result.Results, TaskContinuationOptions.OnlyOnRanToCompletion);
        }

        public Task<IWaitResults> Start(TimeSpan timeout)
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

            
            return WhenReceiveAll();
        }

        private Task<object> ReceiveWithin(TimeSpan maxTimeout, Stopwatch watch)
        {
           return _inbox.ReceiveAsync(maxTimeout - watch.Elapsed)
                        .ContinueWith(t =>
                        {
                            if (t.IsCanceled)
                            {
                                WaitEnd.SetCanceled();
                                throw new TimeoutException();
                            }
                            if (t.IsFaulted)
                            {
                                WaitEnd.SetException(t.Exception);
                                throw t.Exception;
                            }

                            return t.Result;
                        })
                        .ContinueWith(t =>
                        {
                            var currentMessage = t.Result;
                            _allMessages.Add(currentMessage);

                            OnMessageReceived(this, currentMessage);

                            return !WaitIsOver(_allExpectedMessages) ? ReceiveWithin(maxTimeout,watch) : currentMessage;

                        },TaskContinuationOptions.OnlyOnRanToCompletion);
        }


        public void Dispose()
        {
            _inbox.Dispose();
        }
    }
}