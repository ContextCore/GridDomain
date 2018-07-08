using System;

namespace GridDomain.Tests.Common {
    public interface IProcessManagerExpectationBuilder
    {
        IConditionedProcessManagerSender<TMsg> Expect<TMsg>(Predicate<TMsg> filter = null) where TMsg : class; 
    }
}