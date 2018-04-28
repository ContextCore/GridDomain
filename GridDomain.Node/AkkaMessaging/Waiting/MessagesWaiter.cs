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

    public class ExpectedMessageBox 
    {
        public ExpectedMessageBox(ConditionBuilder factory)
        {
            _conditionFactory = factory;
        }
        private readonly ConcurrentBag<object> _allExpectedMessages = new ConcurrentBag<object>();
        private readonly ConditionBuilder _conditionFactory;

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



    public abstract class MessagesWaiter<TBuilder,TWaiter> : IMessageWaiter<TBuilder> where TBuilder : IConditionFactory<TWaiter,TBuilder>
    {
        private readonly TimeSpan _defaultTimeout;
        internal readonly ConditionBuilder ConditionFactory;
        private readonly IActorSubscriber _subscriber;
        private readonly ActorSystem _system;
        private readonly ExpectedMessageBox _expectedMessageBox;

        protected MessagesWaiter(ActorSystem system, IActorSubscriber subscriber, TimeSpan defaultTimeout, ConditionBuilder conditionFactory)
        {
            _system = system;
            _defaultTimeout = defaultTimeout;
            _subscriber = subscriber;
            ConditionFactory = conditionFactory;
            _expectedMessageBox = new ExpectedMessageBox(conditionFactory);
        }

        public abstract TBuilder Expect<TMsg>(Predicate<TMsg> filter = null) where TMsg : class;

        public abstract TBuilder Expect(Type type, Func<object, bool> filter);
       
        public async Task<IWaitResult> Start(TimeSpan? timeout = null)
        {
            if (_expectedMessageBox.AllExpectedMessagesReceived())
                throw new WaiterIsFinishedException();

            using (var inbox = Inbox.Create(_system))
            {
                foreach (var type in ConditionFactory.KnownMessageTypes)
                    await _subscriber.Subscribe(type, inbox.Receiver);

                var finalTimeout = timeout ?? _defaultTimeout;

                await WaitForMessages(inbox, finalTimeout).TimeoutAfter(finalTimeout);

                foreach (var type in ConditionFactory.KnownMessageTypes)
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

    public class MessagesWaiter : MessagesWaiter<IConditionFactory<Task<IWaitResult>>,Task<IWaitResult>>,IMessageWaiter
    {
        private readonly ConditionFactory<Task<IWaitResult>> _conditionFactory;

        public MessagesWaiter(ActorSystem system,
                              IActorSubscriber subscriber,
                              TimeSpan defaultTimeout) : this(system,
                                                              subscriber,
                                                              defaultTimeout,
                                                              new ConditionFactory<Task<IWaitResult>>(new LocalMetadataConditionFactory<Task<IWaitResult>>())) { }

        public MessagesWaiter(ActorSystem system,
                               IActorSubscriber subscriber,
                               TimeSpan defaultTimeout,
                               ConditionFactory<Task<IWaitResult>> conditionFactory) : base(system,
                                                                                            subscriber,
                                                                                            defaultTimeout,
                                                                                            conditionFactory.Builder)
        {
            _conditionFactory = conditionFactory;
            _conditionFactory.CreateResultFunc = Start;
        }

        public override IConditionFactory<Task<IWaitResult>> Expect<TMsg>(Predicate<TMsg> filter = null)
        {
            return _conditionFactory.And(filter);
        }

        public override IConditionFactory<Task<IWaitResult>> Expect(Type type, Func<object, bool> filter)
        {
            return _conditionFactory.And(type, filter);
        }
    }
}