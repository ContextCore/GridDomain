using System;
using GridDomain.CQRS;

namespace GridDomain.Node.AkkaMessaging.Waiting {
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