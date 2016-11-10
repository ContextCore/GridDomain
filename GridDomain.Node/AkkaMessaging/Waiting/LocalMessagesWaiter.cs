using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Threading.Tasks;
using Akka;
using Akka.Actor;
using GridDomain.CQRS;
using GridDomain.CQRS.Messaging.Akka;

namespace GridDomain.Node.AkkaMessaging.Waiting
{
    public abstract class LocalMessagesWaiter<T> : IDisposable, IMessageWaiter<T>
    {
        private readonly IActorSubscriber _subscriber;
        private readonly ConcurrentBag<object> _ignoredMessages = new ConcurrentBag<object>();
        private readonly ConcurrentBag<object> _allExpectedMessages = new ConcurrentBag<object>();

        private readonly List<Func<object,bool>> _filters = new List<Func<object, bool>>();
        private Func<IEnumerable<object>,bool> _stopCondition;

        private readonly Inbox _inbox;
        private readonly TimeSpan _defaultTimeout;
        internal abstract ExpectBuilder<T> ExpectBuilder { get; }

        public LocalMessagesWaiter(ActorSystem system, IActorSubscriber subscriber, TimeSpan defaultTimeout)
        {
            _defaultTimeout = defaultTimeout;
            _subscriber = subscriber;
            _inbox = Inbox.Create(system);
        }

        internal void Subscribe(Type type, 
            Func<object,bool> filter,
            Func<IEnumerable<object>, bool> stopCondition)
        {
            _filters.Add(filter);
            _stopCondition = stopCondition;
            _subscriber.Subscribe(type,_inbox.Receiver);
        }

        public IExpectBuilder<T> Expect<TMsg>(Predicate<TMsg> filter = null)
        {
            return ExpectBuilder.And(filter);
        }
        public IExpectBuilder<T> Expect(Type type, Func<object,bool> filter = null)
        {
            return ExpectBuilder.And(type,filter);
        }

        public async Task<IWaitResults> Start(TimeSpan timeout)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            try
            {
                while (!IsAllExpectedMessagedReceived())
                {
                    try
                    {
                        var message = await _inbox.ReceiveAsync(timeout - stopwatch.Elapsed);
                        CheckExecutionError(message);

                        if (IsExpected(message)) _allExpectedMessages.Add(message);
                        else _ignoredMessages.Add(message);
                    }
                    catch (Exception e)
                    {
                        if(e is ArgumentOutOfRangeException && timeout < stopwatch.Elapsed)
                            throw new TimeoutException();
                        throw;
                    }
                   
                }
                return new WaitResults(_allExpectedMessages);
            }
            finally
            {
                stopwatch.Stop();
                _inbox.Dispose();
            }
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