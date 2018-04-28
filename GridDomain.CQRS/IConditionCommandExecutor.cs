using System;
using System.Threading.Tasks;

namespace GridDomain.CQRS
{
    public interface IConditionCommandExecutor : IMessageFilter<IConditionCommandExecutor>
    {
        Task<IWaitResult> Execute(TimeSpan? timeout = null, bool failOnAnyFault = true);
    }

    public interface IConditionCommandExecutor<T>: IConditionCommandExecutor
    {
        new Task<IWaitResult<T>> Execute(TimeSpan? timeout = null, bool failOnAnyFault = true);
    }
}