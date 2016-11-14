using System;
using System.Threading.Tasks;

namespace GridDomain.CQRS
{
    public interface IMessageWaiterFactory
    {
        IMessageWaiter<Task<IWaitResults>> NewWaiter(TimeSpan? defaultTimeout = null);
        IMessageWaiter<IExpectedCommandExecutor> NewCommandWaiter(TimeSpan? defaultTimeout = null, bool failAnyFault = true);
    }
}