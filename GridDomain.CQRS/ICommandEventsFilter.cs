using System;
using System.Threading.Tasks;

namespace GridDomain.CQRS
{
    public interface ICommandEventsFilter : IMessageFilter<ICommandEventsFilter>
    {
        Task<IWaitResult> Execute(TimeSpan? timeout = null, bool failOnAnyFault = true);
    }

    public interface ICommandEventsFilter<T>: ICommandEventsFilter
    {
        new Task<IWaitResult<T>> Execute(TimeSpan? timeout = null, bool failOnAnyFault = true);
    }
}