using System;
using System.Threading.Tasks;

namespace GridDomain.CQRS
{
    public interface ICommandConditionBuilder
    {
        Task<IWaitResult> Execute(TimeSpan? timeout, bool failOnAnyFault);
        ICommandConditionBuilder And<TMsg>(Predicate<TMsg> filter = null);
        ICommandConditionBuilder Or<TMsg>(Predicate<TMsg> filter = null);
    }

    public interface ICommandConditionBuilder<T>: ICommandConditionBuilder
    {
        new Task<IWaitResult<T>> Execute(TimeSpan? timeout, bool failOnAnyFault);
    }
}