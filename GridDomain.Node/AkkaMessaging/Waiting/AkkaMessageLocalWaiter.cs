using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Threading.Tasks;
using System.Timers;
using Akka;
using Akka.Actor;
using GridDomain.CQRS;
using GridDomain.CQRS.Messaging.Akka;

namespace GridDomain.Node.AkkaMessaging.Waiting
{
    public class AkkaMessageLocalWaiter : IDisposable
    {
        private readonly IActorSubscriber _subscriber;
        private readonly ConcurrentBag<object> _ignoredMessages = new ConcurrentBag<object>();
        private readonly ConcurrentBag<object> _allExpectedMessages = new ConcurrentBag<object>();

        private readonly IDictionary<Type, Predicate<object>> _filters = new Dictionary<Type, Predicate<object>>();
        private Predicate<IEnumerable<object>> _waitIsOver = c => true;

        private readonly Inbox _inbox;
        private readonly ExpectBuilder _expectBuilder;

        public AkkaMessageLocalWaiter(ActorSystem system, IActorSubscriber subscriber)
        {
            _subscriber = subscriber;
            _inbox = Inbox.Create(system);
            _expectBuilder = new ExpectBuilder(this);
        }

        internal void Subscribe<TMsg>(Func<Predicate<IEnumerable<object>>, Predicate<IEnumerable<object>>> waitOverConditionMutator,
                                      Predicate<TMsg> filter)
        {
            _filters[typeof(TMsg)] = o => filter((TMsg)o);
            _waitIsOver = waitOverConditionMutator(_waitIsOver);
            _subscriber.Subscribe<TMsg>(_inbox.Receiver);
        }

        public ExpectBuilder Expect<TMsg>(Predicate<TMsg> filter = null)
        {
            return _expectBuilder.And(filter);
        }
 
        internal bool WasReceived<TMsg>(Predicate<TMsg> filter)
        {
            return _allExpectedMessages.OfType<TMsg>().Any(m => filter(m));
        }

        public Task<IWaitResults> WhenReceiveAll { get; private set; }
        
        public Task<IWaitResults> Start(TimeSpan timeout)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            WhenReceiveAll = ReceiveWithin(timeout, stopwatch)
                                    .ContinueWith(t =>
                                    {
                                        stopwatch.Stop();

                                        //only part of expected messages were received until timeout
                                        if(!IsAllExpectedMessagedReceived())
                                              throw new TimeoutException(); 

                                        return (IWaitResults)new WaitResults(_allExpectedMessages);
                                    },TaskContinuationOptions.OnlyOnRanToCompletion);

            return WhenReceiveAll;
        }

        private Task<object> ReceiveWithin(TimeSpan maxTimeout, Stopwatch watch)
        {
           return _inbox.ReceiveAsync(maxTimeout - watch.Elapsed)
                        .ContinueWith(t =>
                        {
                            CheckExecutionError(t);

                            var message = t.Result;

                            if (IsExpected(message))
                                _allExpectedMessages.Add(message);
                            else
                                _ignoredMessages.Add(message);

                            return !IsAllExpectedMessagedReceived() ? ReceiveWithin(maxTimeout,watch) : message;

                        },TaskContinuationOptions.OnlyOnRanToCompletion);
        }

        private bool IsAllExpectedMessagedReceived()
        {
            return _waitIsOver(_allExpectedMessages);
        }

        private bool IsExpected(object message)
        {
            return _filters.Values.Any(f => f(message));
        }

        private static void CheckExecutionError(Task<object> t)
        {
            if (t.IsCanceled)
            {
                throw new TimeoutException();
            }
            if (t.IsFaulted)
            {
                ExceptionDispatchInfo.Capture(t.Exception).Throw();
            }

            t.Result.Match()
                    .With<Status.Failure>(r => ExceptionDispatchInfo.Capture(r.Cause).Throw())
                    .With<Failure>(r => ExceptionDispatchInfo.Capture(r.Exception).Throw());
        }


        public void Dispose()
        {
            _inbox.Dispose();
        }
    }
}