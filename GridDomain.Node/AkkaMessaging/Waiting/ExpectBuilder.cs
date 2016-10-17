using System;
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
            filter = filter ?? (t => true);
            _waiter.Subscribe(oldPredicate => (c => oldPredicate(c) && _waiter.WasReceived(filter)), filter);
            return this;
        }
        public ExpectBuilder Or<TMsg>(Predicate<TMsg> filter = null)
        {
            filter = filter ?? (t => true);
            _waiter.Subscribe(oldPredicate => (c => oldPredicate(c) || _waiter.WasReceived(filter)), filter);
            return this;
        }

   
    }
}