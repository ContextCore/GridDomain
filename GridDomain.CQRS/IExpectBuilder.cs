using System;
using System.Threading.Tasks;

namespace GridDomain.CQRS
{
    public interface IExpectBuilder<out T>
    {
        T Create();
        IExpectBuilder<T> And<TMsg>(Predicate<TMsg> filter = null);
        IExpectBuilder<T> Or<TMsg>(Predicate<TMsg> filter = null);
    }

    public interface ICommandExpectBuilder
    {
        Task<IWaitResults> Execute(TimeSpan? timeout = null, bool failOnAnyFault = true);
        ICommandExpectBuilder And<TMsg>(Predicate<TMsg> filter = null);
        ICommandExpectBuilder Or<TMsg>(Predicate<TMsg> filter = null);
    }
}