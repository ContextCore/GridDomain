using System;

namespace GridDomain.CQRS
{
    public interface IMessageWaiterBase<TResult, TExpectBuilder> where TExpectBuilder : IExpectBuilder<TResult>
    {
        TExpectBuilder Expect<TMsg>(Predicate<TMsg> filter = null);
        TExpectBuilder Expect(Type type, Func<object, bool> filter = null);
    }
}