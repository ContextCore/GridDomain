using System;

namespace GridDomain.CQRS
{
    public interface IExpectBuilder<out T>
    {
        T Create();
        IExpectBuilder<T> And<TMsg>(Predicate<TMsg> filter = null);
        IExpectBuilder<T> Or<TMsg>(Func<TMsg, bool> filter = null);
    }
}