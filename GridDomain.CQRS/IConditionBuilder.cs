using System;

namespace GridDomain.CQRS
{
    public interface IConditionBuilder<out T>
    {
        T Create();
        IConditionBuilder<T> And<TMsg>(Predicate<TMsg> filter = null);
        IConditionBuilder<T> Or<TMsg>(Predicate<TMsg> filter = null);
    }
}