using System;

namespace GridDomain.CQRS
{
    public interface IMessageWaiterBase<TResult, out TExpectBuilder> where TExpectBuilder : IConditionBuilder<TResult>
    {
        TExpectBuilder Expect<TMsg>(Predicate<TMsg> filter = null);
     //   TExpectBuilder Expect(Type type, Func<object, bool> filter = null);
    }
}