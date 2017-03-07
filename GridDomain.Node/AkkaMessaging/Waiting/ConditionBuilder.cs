using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using GridDomain.Common;
using GridDomain.CQRS;

namespace GridDomain.Node.AkkaMessaging.Waiting
{
    /// <summary>
    /// Works with messages sent in metadata envelop
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class MetadataConditionBuilder<T> : ConditionBuilder<T>
    {
        protected override bool ApplyDefaultFilter<TMsg>(object message)
        {
            return message is IMessageMetadataEnvelop<TMsg>;
        }

        protected override bool ApplyDefinedFilter<TMsg>(object message, Predicate<TMsg> filter)
        {
            var envelop = message as IMessageMetadataEnvelop<TMsg>;
            return envelop != null && filter(envelop.Message);
        }
    }

    /// <summary>
    ///     Create expectation condition
    ///     Each opertion is placed in brackets after definition.
    ///     E.G.
    ///     ExpectBuilder.And(A).Or(B).And(C) converts into ((A or B) and C)
    ///     and
    ///     ExpectBuilder.And(A).And(B).Or(C).And(D) into (((A and B) or C) and D)
    /// </summary>
    /// <typeparam name="TMsg"></typeparam>
    /// <param name="filter"></param>
    /// <returns></returns>
    public class ConditionBuilder<T> : IConditionBuilder<T>
    {
        private Expression<Func<IEnumerable<object>, bool>> StopExpression { get; set; } = c => true;
        public Func<IEnumerable<object>, bool> StopCondition { get; private set; }
        public IDictionary<Type, Func<object, bool>> MessageFilters = new Dictionary<Type, Func<object, bool>>();
        internal Func<T> CreateResultFunc;

        public T Create()
        {
            StopCondition = StopExpression.Compile();
            return CreateResultFunc.Invoke();
        }
        public ConditionBuilder(Func<T> createResultFunc = null)
        {
            CreateResultFunc = createResultFunc;
        }

        public IConditionBuilder<T> And<TMsg>(Predicate<TMsg> filter = null)
        {
            return filter == null
                       ? And(typeof(TMsg), ApplyDefaultFilter<TMsg>)
                       : And(typeof(TMsg), o => ApplyDefinedFilter(o, filter));
        }

        public IConditionBuilder<T> Or<TMsg>(Predicate<TMsg> filter = null)
        {
            return filter == null
                       ? Or(typeof(TMsg), ApplyDefaultFilter<TMsg>)
                       : Or(typeof(TMsg), o => ApplyDefinedFilter(o, filter));
        }


        protected virtual bool ApplyDefaultFilter<TMsg>(object message)
        {
            return message is TMsg;
        }

        protected virtual bool ApplyDefinedFilter<TMsg>(object message, Predicate<TMsg> filter)
        {
            return message is TMsg && filter((TMsg) message);
        }

        public IConditionBuilder<T> And(Type type, Func<object, bool> filter)
        {
            StopExpression = StopExpression.And(c => c != null && c.Any(filter));
            MessageFilters[type] = filter;
            return this;
        }

        public IConditionBuilder<T> Or(Type type, Func<object, bool> filter)
        {
            StopExpression = StopExpression.Or(c => c != null && c.Any(filter));
            MessageFilters[type] = filter;
            return this;
        }
    }
}