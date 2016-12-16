using System;
using System.Threading.Tasks;
using GridDomain.Common;

namespace GridDomain.CQRS
{
    public interface IMessageWaiterFactory
    {
        IMessageWaiter<Task<IWaitResults>> NewWaiter(TimeSpan? defaultTimeout = null);
        IMessageWaiter<IExpectedCommandExecutor> NewCommandWaiter(TimeSpan? defaultTimeout = null, bool failAnyFault = true);
    }

    public interface ICommandWaiterFactory
    {
        ICommandWaiter PrepareCommand(ICommand cmd);
    }

    public interface ICommandWaiter : IMessageWaiter<Task<IWaitResults>>
    {
        Task<IWaitResults> Execute(TimeSpan? timeout = null, bool failOnAnyFault = true);
    }

    // GridNode.PrepareCommand(cmd)
    //         .Expect<A>
    //         .Execute(timeout, null)
}