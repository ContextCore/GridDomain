using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Akka.Persistence;
using Google.Protobuf;
using GridDomain.Common;
using GridDomain.CQRS;

namespace GridDomain.Node.AkkaMessaging.Waiting
{
    public interface IMessagesExpectation
    {
        bool IsExpected(object message);
        bool IsExpectationFulfilled(IReadOnlyCollection<object> messages);
        IReadOnlyCollection<Type> ExpectedMessageTypes { get; }
    }

    /// <summary>
    ///     Create expectation condition
    ///     Each opertion is placed in brackets after definition.
    ///     E.G.
    ///     ExpectBuilder.And(A).Or(B).And(C) converts into ((A or B) and C)
    ///     and
    ///     ExpectBuilder.And(A).And(B).Or(C).And(D) into (((A and B) or C) and D)
    /// </summary>
    public class ConditionBuilder
    {
        class DelegateMessagesExpectation : IMessagesExpectation
        {
            private readonly Predicate<object> _singleMessage;
            private readonly Predicate<IReadOnlyCollection<object>> _severalMessages;

            public DelegateMessagesExpectation(Predicate<object> singleMessage,
                                               Predicate<IReadOnlyCollection<object>> severalMessages,
                                               IReadOnlyCollection<Type> expectedTypes)
            {
                _severalMessages = severalMessages;
                _singleMessage = singleMessage;
                ExpectedMessageTypes = expectedTypes;
            }

            public bool IsExpected(object message) => _singleMessage(message);

            public bool IsExpectationFulfilled(IReadOnlyCollection<object> messages) => _severalMessages(messages);
            public IReadOnlyCollection<Type> ExpectedMessageTypes { get; }
        }

        private Expression<Func<IEnumerable<object>, bool>> StopExpression { get; set; } = c => true;

        private Func<IEnumerable<object>, bool> StopCondition { get; set; }

        protected readonly List<Func<object, bool>> MessageFilters = new List<Func<object, bool>>();
        protected readonly List<Type> AcceptedMessageTypes = new List<Type>();

        public ConditionBuilder And<TMsg>(Predicate<TMsg> filter = null) where TMsg : class
        {
            return filter == null
                       ? And(typeof(TMsg))
                       : And(typeof(TMsg), o => filter((TMsg) o));
        }

        public ConditionBuilder Or<TMsg>(Predicate<TMsg> filter = null) where TMsg : class
        {
            return filter == null
                       ? Or(typeof(TMsg))
                       : Or(typeof(TMsg), o => filter((TMsg) o));
        }

        private bool IsExpected(object message)
        {
            return MessageFilters.Any(f => f(message));
        }

        private bool IsConditionFulfilled(IReadOnlyCollection<object> messages)
        {
            return StopCondition(messages);
        }

        public ConditionBuilder And(Type messageType, Func<object, bool> filter = null)
        {
            var filterWithAdapter = AddFilter(messageType, filter);
            StopExpression = StopExpression.And(c => c != null && c.Any(filterWithAdapter));

            return this;
        }

        public ConditionBuilder Or(Type type, Func<object, bool> filter = null)
        {
            var filterWithAdapter = AddFilter(type, filter);
            StopExpression = StopExpression.Or(c => c != null && c.Any(filterWithAdapter));
            return this;
        }

        public IMessagesExpectation Build()
        {
            StopCondition = StopExpression.Compile();
            return new DelegateMessagesExpectation(m => IsExpected(m), m => IsConditionFulfilled(m), AcceptedMessageTypes);
        }

        protected virtual bool CheckMessageType(object receivedMessage, Type t, Func<object, bool> domainMessageFilter = null)
        {
            if (domainMessageFilter == null)
                return t.IsInstanceOfType(receivedMessage);
            
            return t.IsInstanceOfType(receivedMessage) && domainMessageFilter(receivedMessage);
        }

        protected virtual Func<object, bool> AddFilter(Type messageType, Func<object, bool> filter = null)
        {
            Func<object, bool> filterWithAdapter = (o => CheckMessageType(o, messageType, filter));
            AcceptedMessageTypes.Add(messageType);
            MessageFilters.Add(o => filterWithAdapter(o));
            return filterWithAdapter;
        }
    }

    public class MessageConditionFactory<T> : IMessageConditionFactory<T>
    {
        public Func<TimeSpan?, T> CreateResultFunc;
        public readonly ConditionBuilder Builder;

        public T Create()
        {
            return Create(null);
        }

        public T Create(TimeSpan? timeout)
        {
            return CreateResultFunc.Invoke(timeout);
        }

        public MessageConditionFactory(ConditionBuilder builder = null, Func<TimeSpan?, T> createResultFunc = null)
        {
            Builder = builder ?? new ConditionBuilder();
            CreateResultFunc = createResultFunc;
        }

        public IMessageConditionFactory<T> And<TMsg>(Predicate<TMsg> filter = null) where TMsg : class
        {
            Builder.And(filter);
            return this;
        }

        public IMessageConditionFactory<T> Or<TMsg>(Predicate<TMsg> filter = null) where TMsg : class
        {
            Builder.Or(filter);
            return this;
        }

        public IMessageConditionFactory<T> And(Type type, Func<object, bool> filter = null)
        {
            Builder.And(type, filter);
            return this;
        }

        public IMessageConditionFactory<T> Or(Type type, Func<object, bool> filter = null)
        {
            Builder.Or(type, filter);
            return this;
        }
    }
}