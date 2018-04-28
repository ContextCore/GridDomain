using System;
using System.Threading.Tasks;

namespace GridDomain.CQRS
{
    public interface ICommandFilter : IMessageFilter<ICommandFilter>
    {
        Task<IWaitResult> Execute(TimeSpan? timeout = null, bool failOnAnyFault = true);
    }

    public interface ICommandFilter<T>: ICommandFilter
    {
        new Task<IWaitResult<T>> Execute(TimeSpan? timeout = null, bool failOnAnyFault = true);
    }
}