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
        public ExpectedMessageBox(IMessagesExpectation expectation)
        {
            _conditionExpectation = expectation;
        }

        private readonly ConcurrentBag<object> _allExpectedMessages = new ConcurrentBag<object>();
        private readonly IMessagesExpectation _conditionExpectation;

        public bool Receive(object message)
        {
            if (!_conditionExpectation.IsExpected(message)) return false;

            _allExpectedMessages.Add(message);
            return true;
        }

        public bool AllExpectedMessagesReceived()
        {
            return _conditionExpectation.IsExpectationFulfilled(_allExpectedMessages);
        }

        public IReadOnlyCollection<object> ReceivedMessages => _allExpectedMessages;
    }


    public abstract class MessagesWaiter<TBuilder> : IMessageWaiter<TBuilder> where TBuilder : IMessageFilter<TBuilder>
    {
        private readonly TimeSpan _defaultTimeout;
        private readonly IActorSubscriber _subscriber;
        private readonly ActorSystem _system;
        private ExpectedMessageBox _expectedMessageBox;

        protected MessagesWaiter(ActorSystem system, IActorSubscriber subscriber, TimeSpan defaultTimeout)
        {
            _system = system;
            _defaultTimeout = defaultTimeout;
            _subscriber = subscriber;
        }

        public abstract IMessagesExpectation CreateMessagesExpectation();
        public abstract TBuilder Expect<TMsg>(Predicate<TMsg> filter = null) where TMsg : class;
        public abstract TBuilder Expect(Type type, Func<object, bool> filter);

        public async Task<IWaitResult> Start(TimeSpan? timeout = null)
        {
            if (_expectedMessageBox != null)
                throw new WaiterIsFinishedException();

            var messagesExpectation = CreateMessagesExpectation();
            _expectedMessageBox = new ExpectedMessageBox(messagesExpectation);

            using (var inbox = Inbox.Create(_system))
            {
                foreach (var type in messagesExpectation.ExpectedMessageTypes)
                    await _subscriber.Subscribe(type, inbox.Receiver);

                var finalTimeout = timeout ?? _defaultTimeout;

                await WaitForMessages(inbox, finalTimeout)
                    .TimeoutAfter(finalTimeout);

                foreach (var type in messagesExpectation.ExpectedMessageTypes)
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

    public class MessagesWaiter : MessagesWaiter<IMessageConditionFactory<Task<IWaitResult>>>, IMessageWaiter
    {
        private readonly MessageConditionFactory<Task<IWaitResult>> _messageConditionFactory;

        public MessagesWaiter(ActorSystem system,
                              IActorSubscriber subscriber,
                              TimeSpan defaultTimeout) : this(system,
                                                              subscriber,
                                                              defaultTimeout,
                                                              new MessageConditionFactory<Task<IWaitResult>>(new LocalMetadataEnvelopConditionBuilder())) { }

        public MessagesWaiter(ActorSystem system,
                              IActorSubscriber subscriber,
                              TimeSpan defaultTimeout,
                              MessageConditionFactory<Task<IWaitResult>> messageConditionFactory) : base(system,
                                                                                                         subscriber,
                                                                                                         defaultTimeout)
        {
            _messageConditionFactory = messageConditionFactory;
            _messageConditionFactory.CreateResultFunc = Start;
        }

        public override IMessagesExpectation CreateMessagesExpectation()
        {
            return _messageConditionFactory.Builder.Build();
        }

        public override IMessageConditionFactory<Task<IWaitResult>> Expect<TMsg>(Predicate<TMsg> filter = null)
        {
            return _messageConditionFactory.And(filter);
        }

        public override IMessageConditionFactory<Task<IWaitResult>> Expect(Type type, Func<object, bool> filter)
        {
            return _messageConditionFactory.And(type, filter);
        }
    }
}