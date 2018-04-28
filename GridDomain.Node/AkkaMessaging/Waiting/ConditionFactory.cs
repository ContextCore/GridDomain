using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Akka.Persistence;
using GridDomain.Common;
using GridDomain.CQRS;

namespace GridDomain.Node.AkkaMessaging.Waiting
{
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
        private Expression<Func<IEnumerable<object>, bool>> StopExpression { get; set; } = c => true;
        public Func<IEnumerable<object>, bool> StopCondition { get; private set; }

        //message filter should be able to proceed message with type from filter key
        private readonly IDictionary<Type, List<Func<object, bool>>> _messageFilters = new Dictionary<Type, List<Func<object, bool>>>();
        
        private readonly HashSet<Type> _knownMessageTypes = new HashSet<Type>();
      

        public ConditionBuilder And<TMsg>(Predicate<TMsg> filter = null) where TMsg : class
        {
            return filter == null
                       ? And(typeof(TMsg), DefaultFilter<TMsg>)
                       : And(typeof(TMsg), o => DomainFilterAdapter(o, filter));
        }

        public ConditionBuilder Or<TMsg>(Predicate<TMsg> filter = null) where TMsg : class
        {
            return filter == null
                       ? Or(typeof(TMsg), DefaultFilter<TMsg>)
                       : Or(typeof(TMsg),o => DomainFilterAdapter(o,filter));
        }

        public IReadOnlyCollection<Type> KnownMessageTypes => _knownMessageTypes;

        public bool Check(params object[] messages)
        {
            var allFilters = _messageFilters.SelectMany(v => v.Value).ToArray();
            return messages.All(m => allFilters.Any(filter => filter(m)));
        }

        public ConditionBuilder And(Type type, Func<object, bool> filter = null)
        {
            var messageFilter = filter ?? DefaultFilter<object>;
            StopExpression = StopExpression.And(c => c != null && c.Any(messageFilter));
            StopCondition = StopExpression.Compile();

            AddFilter(type, messageFilter);
            return this;
        }

        public ConditionBuilder Or(Type type, Func<object, bool> filter = null)
        {
            var messageFilter = filter ?? DefaultFilter<object>;
            StopExpression = StopExpression.Or(c => c != null && c.Any(messageFilter));
            StopCondition = StopExpression.Compile();

            AddFilter(type, messageFilter);
            return this;
        }

        protected virtual bool DefaultFilter<TMsg>(object message)
        {
            return message is TMsg;
        }

        protected virtual bool DomainFilterAdapter<TMsg>(object receivedMessage, Predicate<TMsg> domainMessageFilter) where TMsg : class
        {
            return receivedMessage is TMsg msg && domainMessageFilter(msg);
        }
       

        protected virtual void AddFilter(Type type, Func<object, bool> filter)
        {
            Condition.NotNull(() => filter);
            Condition.NotNull(() => type);

            _knownMessageTypes.Add(type);

            if (!_messageFilters.TryGetValue(type, out var list))
                list = _messageFilters[type] = new List<Func<object, bool>>();

            list.Add(filter);
        }
    }
   
   
    public class ConditionFactory<T> : IConditionFactory<T>
    {
        public Func<TimeSpan?, T> CreateResultFunc;
        public readonly ConditionBuilder Builder;

        public T Create()
        {
            return Create(null);
        }

        public IReadOnlyCollection<Type> RequiredMessageTypes => KnownMessageTypes;

        public T Create(TimeSpan? timeout)
        {
            return CreateResultFunc.Invoke(timeout);
        }

        public ConditionFactory(ConditionBuilder builder=null,Func<TimeSpan?, T> createResultFunc = null)
        {
            Builder = builder;
            CreateResultFunc = createResultFunc;
        }

        public IConditionFactory<T> And<TMsg>(Predicate<TMsg> filter = null) where TMsg : class
        {
            Builder.And(filter);
            return this;
        }

        public IConditionFactory<T> Or<TMsg>(Predicate<TMsg> filter = null) where TMsg : class
        {
            Builder.Or(filter);
            return this;
        }

        public IReadOnlyCollection<Type> KnownMessageTypes => Builder.KnownMessageTypes;

        public bool Check(params object[] messages) => Builder.Check(messages);

        public IConditionFactory<T> And(Type type, Func<object, bool> filter = null)
        {
            Builder.And(type, filter);
            return this;
        }

        public IConditionFactory<T> Or(Type type, Func<object, bool> filter = null)
        {
            Builder.Or(type, filter);
            return this;
        }

        public bool StopCondition(IEnumerable<object> allExpectedMessages) => Builder.StopCondition(allExpectedMessages);

    }
}