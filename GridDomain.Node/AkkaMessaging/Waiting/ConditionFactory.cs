using System;
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
    /// <typeparam name="T">type for return on Create method to better chaining</typeparam>
    /// <returns></returns>
    public class ConditionFactory<T> : IConditionFactory<T>
    {
        private Expression<Func<IEnumerable<object>, bool>> StopExpression { get; set; } = c => true;
        public Func<IEnumerable<object>, bool> StopCondition { get; private set; }

        //message filter should be able to proceed message with type from filter key
        private readonly IDictionary<Type, List<Func<object, bool>>> _messageFilters = new Dictionary<Type, List<Func<object, bool>>>();
        
        private readonly HashSet<Type> _knownMessageTypes = new HashSet<Type>();
        
        public Func<TimeSpan?, T> CreateResultFunc;

        public T Create()
        {
            return Create(null);
        }

        public IReadOnlyCollection<Type> RequiredMessageTypes => KnownMessageTypes;

        public T Create(TimeSpan? timeout)
        {
            return CreateResultFunc.Invoke(timeout);
        }

        public ConditionFactory(Func<TimeSpan?, T> createResultFunc = null)
        {
            CreateResultFunc = createResultFunc;
        }

        public IConditionFactory<T> And<TMsg>(Predicate<TMsg> filter = null) where TMsg : class
        {
            return filter == null
                       ? And(typeof(TMsg), DefaultFilter<TMsg>)
                       : And(typeof(TMsg), o => DomainFilterAdapter(o, filter));
        }

        public IConditionFactory<T> Or<TMsg>(Predicate<TMsg> filter = null) where TMsg : class
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

        public IConditionFactory<T> And(Type type, Func<object, bool> filter = null)
        {
            var messageFilter = filter ?? DefaultFilter<object>;
            StopExpression = StopExpression.And(c => c != null && c.Any(messageFilter));
            StopCondition = StopExpression.Compile();

            AddFilter(type, messageFilter);
            return this;
        }

        public IConditionFactory<T> Or(Type type, Func<object, bool> filter = null)
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
}