using System;

namespace GridDomain.Node.AkkaMessaging.Waiting
{
    public interface IExpectBuilder<out T>
    {
        T Create(TimeSpan timeout);
        IExpectBuilder<T> And<TMsg>(Predicate<TMsg> filter = null);
        IExpectBuilder<T> Or<TMsg>(Func<TMsg, bool> filter = null);
    }
}