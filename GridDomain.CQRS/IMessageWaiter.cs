using System;

namespace GridDomain.CQRS
{
    public interface IMessageWaiter<T>
    {
        IExpectBuilder<T> Expect<TMsg>(Predicate<TMsg> filter = null);
        IExpectBuilder<T> Expect(Type type, Func<object, bool> filter = null);
    }
}