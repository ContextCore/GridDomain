using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
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

        private readonly List<Func<object,bool>> _filters = new List<Func<object, bool>>();
        private Func<IEnumerable<object>,bool> _stopCondition;

        private readonly Inbox _inbox;
        internal readonly ExpectBuilder ExpectBuilder;

        public AkkaMessageLocalWaiter(ActorSystem system, IActorSubscriber subscriber)
        {
            _subscriber = subscriber;
            _inbox = Inbox.Create(system);
            ExpectBuilder = new ExpectBuilder(this);
        }

        internal void Subscribe(Type type, 
                                Func<object,bool> filter,
                                Func<IEnumerable<object>, bool> stopCondition)
        {
            _filters.Add(filter);
            _stopCondition = stopCondition;
            _subscriber.Subscribe(type,_inbox.Receiver);
        }

        public ExpectBuilder Expect<TMsg>(Predicate<TMsg> filter = null)
        {
            return ExpectBuilder.And(filter);
        }
        public ExpectBuilder Expect(Type type, Func<object,bool> filter = null)
        {
            return ExpectBuilder.And(type,filter);
        }

        public Task<IWaitResults> WhenReceiveAll { get; private set; }
        
        public Task<IWaitResults> Start(TimeSpan timeout)
        {
            return WhenReceiveAll = Task.Run(() =>
            {
                var stopwatch = new Stopwatch();
                stopwatch.Start();
                try
                {
                    while (!IsAllExpectedMessagedReceived())
                    {
                        var message = _inbox.Receive(timeout - stopwatch.Elapsed);
                        CheckExecutionError(message);

                        if (IsExpected(message))
                            _allExpectedMessages.Add(message);
                        else
                            _ignoredMessages.Add(message);
                    }

                    return (IWaitResults) new WaitResults(_allExpectedMessages);
                }
                finally
                {
                    stopwatch.Stop();
                }
            });
        }

        private bool IsAllExpectedMessagedReceived()
        {
            return _stopCondition(_allExpectedMessages);
        }

        private bool IsExpected(object message)
        {
            return _filters.Any(f => f(message));
        }

        private static void CheckExecutionError(object t)
        {
            //if (t.IsCanceled)
            //    throw new TimeoutException();
            //
            //if (t.IsFaulted)
            //    ExceptionDispatchInfo.Capture(t.Exception).Throw();

            t.Match()
             .With<Status.Failure>(r => ExceptionDispatchInfo.Capture(r.Cause).Throw())
             .With<Failure>(r => ExceptionDispatchInfo.Capture(r.Exception).Throw());
        }


        public void Dispose()
        {
            _inbox.Dispose();
        }
    }
}