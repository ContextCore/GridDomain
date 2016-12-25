using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using GridDomain.Common;
using GridDomain.CQRS;

namespace GridDomain.Node.AkkaMessaging.Waiting
{
    /// <summary>
    /// Create expectation condition
    /// Each opertion is placed in brackets after definition.
    /// E.G. 
    /// ExpectBuilder.And(A).Or(B).And(C) converts into ((A or B) and C)
    /// and 
    /// ExpectBuilder.And(A).And(B).Or(C).And(D) into (((A and B) or C) and D)  
    /// </summary>
    /// <typeparam name="TMsg"></typeparam>
    /// <param name="filter"></param>
    /// <returns></returns>
    public abstract class ExpectBuilder<T> : IExpectBuilder<T>
    {
        protected readonly LocalMessagesWaiter<T> Waiter;
        internal Expression<Func<IEnumerable<object>, bool>> WaitIsOver = c => true;

        protected ExpectBuilder(LocalMessagesWaiter<T> waiter)
        {
            Waiter = waiter;
        }

        public abstract T Create(TimeSpan? timeout);
        public T Create()
        {
            return Create(null);
        }

        private bool MessageHasCorrectType<TMsg>(object message)
        {
            return message is TMsg || message is IMessageMetadataEnvelop<TMsg>;
        }
        private bool MessagePassFilter<TMsg>(object message, Predicate<TMsg> filter)
        {
            return (message is TMsg && filter((TMsg)message))
                || (message is IMessageMetadataEnvelop<TMsg> && filter(((IMessageMetadataEnvelop<TMsg>)message).Message));
        }


        public IExpectBuilder<T> And<TMsg>(Predicate<TMsg> filter = null)
        {

            return filter == null ? And(typeof(TMsg), MessageHasCorrectType<TMsg>) :
                                    And(typeof(TMsg), o => MessagePassFilter(o, filter));
        }

        public IExpectBuilder<T> Or<TMsg>(Predicate<TMsg> filter = null)
        {
            return filter == null ? Or(typeof(TMsg), MessageHasCorrectType<TMsg>) :
                                    Or(typeof(TMsg), o => MessagePassFilter(o, filter));
        }

        public IExpectBuilder<T> And(Type type,Func<object,bool> filter)
        {
            WaitIsOver = WaitIsOver.And(c => c != null && c.Any(filter));
            Waiter.Subscribe(type, filter, WaitIsOver.Compile());
            Waiter.Subscribe(MessageMetadataEnvelop.GenericForType(type), filter, WaitIsOver.Compile());
            return this;
        }

        public IExpectBuilder<T> Or(Type type, Func<object,bool> filter)
        {
            WaitIsOver = WaitIsOver.Or(c => c != null && c.Any(filter));
            Waiter.Subscribe(type, filter, WaitIsOver.Compile());
            Waiter.Subscribe(MessageMetadataEnvelop.GenericForType(type), filter, WaitIsOver.Compile());
            return this;
        }
    }
}