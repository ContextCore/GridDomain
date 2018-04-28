using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.ExceptionServices;
using System.Threading.Tasks;
using Akka;
using Akka.Actor;
using GridDomain.Common;
using GridDomain.CQRS;
using GridDomain.Transport;
using Serilog;

namespace GridDomain.Node.AkkaMessaging.Waiting
{

    public class ExpectedMessageBox<T> 
    {
        public ExpectedMessageBox(ConditionFactory<T> factory)
        {
            _conditionFactory = factory;
        }
        private readonly ConcurrentBag<object> _allExpectedMessages = new ConcurrentBag<object>();
        private readonly ConditionFactory<T> _conditionFactory;

        public bool Receive(object message)
        {
            if (!_conditionFactory.Check(message)) return false;
            
            _allExpectedMessages.Add(message);
            return true;
        }

        public bool AllExpectedMessagesReceived()
        {
            return _conditionFactory.StopCondition(_allExpectedMessages);
        }

        public IReadOnlyCollection<object> ReceivedMessages => _allExpectedMessages;
    }



    public class MessagesWaiter<T> : IMessageWaiter<T>
    {
        private readonly TimeSpan _defaultTimeout;
        internal readonly ConditionFactory<T> ConditionFactory;
        private readonly IActorSubscriber _subscriber;
        private readonly ActorSystem _system;
        private readonly ExpectedMessageBox<T> _expectedMessageBox;

        public MessagesWaiter(ActorSystem system, IActorSubscriber subscriber, TimeSpan defaultTimeout, ConditionFactory<T> conditionFactory)
        {
            _system = system;
            _defaultTimeout = defaultTimeout;
            _subscriber = subscriber;
            ConditionFactory = conditionFactory;
            _expectedMessageBox = new ExpectedMessageBox<T>(conditionFactory);
        }

        public IConditionFactory<T> Expect<TMsg>(Predicate<TMsg> filter = null) where TMsg : class
        {
            return ConditionFactory.And(filter);
        }

        public IConditionFactory<T> Expect(Type type, Func<object, bool> filter)
        {
            return ConditionFactory.And(type, filter);
        }

        public async Task<IWaitResult> Start(TimeSpan? timeout = null)
        {
            if (_expectedMessageBox.AllExpectedMessagesReceived())
                throw new WaiterIsFinishedException();

            using (var inbox = Inbox.Create(_system))
            {
                foreach (var type in ConditionFactory.RequiredMessageTypes)
                    await _subscriber.Subscribe(type, inbox.Receiver);

                var finalTimeout = timeout ?? _defaultTimeout;

                await WaitForMessages(inbox, finalTimeout).TimeoutAfter(finalTimeout);

                foreach (var type in ConditionFactory.RequiredMessageTypes)
                    await _subscriber.Unsubscribe(inbox.Receiver, type);

                return new WaitResult(_expectedMessageBox.ReceivedMessages);
            }
        }

        private async Task WaitForMessages(Inbox inbox, TimeSpan timeoutPerMessage)
        {
            do
            {
                var message = await inbox.ReceiveAsync(timeoutPerMessage)
                                         .ConfigureAwait(false);
                
                CheckExecutionError(message);
                _expectedMessageBox.Receive(message);
            }
            while (!_expectedMessageBox.AllExpectedMessagesReceived());
        }

        private static void CheckExecutionError(object t)
        {
            t.Match()
             .With<Status.Failure>(r => throw r.Cause)
             .With<Failure>(r => throw r.Exception);
        }
    }

    public class MessagesWaiter : MessagesWaiter<Task<IWaitResult>>
    {
        public MessagesWaiter(ActorSystem system,
                              IActorSubscriber subscriber,
                              TimeSpan defaultTimeout) : this(system,
                                                              subscriber,
                                                              defaultTimeout,
                                                              new LocalMetadataConditionFactory<Task<IWaitResult>>()) { }

        private MessagesWaiter(ActorSystem system,
                               IActorSubscriber subscriber,
                               TimeSpan defaultTimeout,
                               ConditionFactory<Task<IWaitResult>> conditionFactory) : base(system,
                                                                                            subscriber,
                                                                                            defaultTimeout,
                                                                                            conditionFactory)
        {
            conditionFactory.CreateResultFunc = Start;
        }
    }
}