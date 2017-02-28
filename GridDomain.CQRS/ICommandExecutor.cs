using GridDomain.Common;

namespace GridDomain.CQRS
{
    public interface ICommandExecutor
    {
        void Execute<T>(T command, IMessageMetadata metadata = null) where T : ICommand;
        ICommandWaiter Prepare<T>(T cmd, IMessageMetadata metadata = null) where T : ICommand;
    }
}