using GridDomain.Common;

namespace GridDomain.CQRS
{
    public interface ICommandWaiterFactory
    {
        ICommandWaiter PrepareCommand<T>(T cmd, IMessageMetadata metadata=null) where T:ICommand;
    }
}