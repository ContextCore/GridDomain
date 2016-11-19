using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
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

        public IExpectBuilder<T> And<TMsg>(Predicate<TMsg> filter = null)
        {
            return filter == null ? And(typeof(TMsg), o => o is TMsg) :
                                    And(typeof(TMsg), o => o is TMsg && filter((TMsg)o));
        }

        public IExpectBuilder<T> Or<TMsg>(Func<TMsg, bool> filter = null)
        {
            return filter == null ? Or(typeof(TMsg), o => o is TMsg) :
                                    Or(typeof(TMsg), o => o is TMsg && filter((TMsg)o));
        }

        public IExpectBuilder<T> And(Type type,Func<object,bool> filter)
        {
            WaitIsOver = WaitIsOver.And(c => c.Any(filter));
            Waiter.Subscribe(type, filter, WaitIsOver.Compile());
            return this;
        }

        public IExpectBuilder<T> Or(Type type, Func<object,bool> filter)
        {
            WaitIsOver = WaitIsOver.Or(c => c.Any(filter));
            Waiter.Subscribe(type,filter, WaitIsOver.Compile());
            return this;
        }
    }
}