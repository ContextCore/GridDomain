using System;
using System.Threading.Tasks;

namespace GridDomain.CQRS
{
    public interface ICommandConditionBuilder
    {
        Task<IWaitResult> Execute(TimeSpan? timeout = null, bool failOnAnyFault = true);
        ICommandConditionBuilder And<TMsg>(Predicate<TMsg> filter = null) where TMsg : class;
        ICommandConditionBuilder Or<TMsg>(Predicate<TMsg> filter = null) where TMsg : class;
    }

    public interface ICommandConditionBuilder<T>: ICommandConditionBuilder
    {
        new Task<IWaitResult<T>> Execute(TimeSpan? timeout = null, bool failOnAnyFault = true);
    }
}