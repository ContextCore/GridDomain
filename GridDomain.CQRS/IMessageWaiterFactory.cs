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
        ICommandWaiter PrepareCommand<T>(T cmd, IMessageMetadata metadata=null) where T:ICommand;
    }

    public interface ICommandWaiter
    {
        ICommandExpectBuilder Expect<TMsg>(Predicate<TMsg> filter = null);
        ICommandExpectBuilder Expect(Type type, Func<object, bool> filter = null);
    }
}