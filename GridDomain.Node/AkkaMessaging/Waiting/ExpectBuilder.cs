using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

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
    public class ExpectBuilder
    {
        private readonly AkkaMessageLocalWaiter _waiter;
        internal Expression<Func<IEnumerable<object>, bool>> WaitIsOver = c => true;
        public ExpectBuilder(AkkaMessageLocalWaiter waiter)
        {
            _waiter = waiter;
        }

        public Task<IWaitResults> Within(TimeSpan timeout)
        {
            return _waiter.Start(timeout);
        }
 
        public ExpectBuilder And<TMsg>(Predicate<TMsg> filter = null)
        {
            return filter == null ? And(typeof(TMsg), o => o is TMsg) :
                                    And(typeof(TMsg), o => o is TMsg && filter((TMsg)o));
        }
        public ExpectBuilder Or<TMsg>(Func<TMsg, bool> filter = null)
        {
            return filter == null ? Or(typeof(TMsg), o => o is TMsg) :
                                    Or(typeof(TMsg), o => o is TMsg && filter((TMsg)o));
        }

        public ExpectBuilder And(Type type,Func<object,bool> filter)
        {
            WaitIsOver = WaitIsOver.And(c => c.Any(filter));
            _waiter.Subscribe(type, filter, WaitIsOver.Compile());
            return this;
        }

        public ExpectBuilder Or(Type type, Func<object,bool> filter)
        {
            WaitIsOver = WaitIsOver.Or(c => c.Any(filter));
            _waiter.Subscribe(type,filter, WaitIsOver.Compile());
            return this;
        }
    }
}