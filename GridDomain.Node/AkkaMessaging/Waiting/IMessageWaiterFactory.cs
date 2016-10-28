using System;
using System.Threading.Tasks;

namespace GridDomain.Node.AkkaMessaging.Waiting
{
    public interface IMessageWaiterFactory
    {
        IMessageWaiter<Task<IWaitResults>>  NewWaiter(TimeSpan? defaultTimeout = null);
        IMessageWaiter<IExpectedCommandExecutor>  NewCommandWaiter(TimeSpan? defaultTimeout = null);
    }
}