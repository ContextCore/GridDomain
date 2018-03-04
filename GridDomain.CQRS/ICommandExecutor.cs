using System.Threading.Tasks;
using GridDomain.Common;

namespace GridDomain.CQRS
{
    public enum CommandConfirmationMode
    {
           None,
           Executed,
           Projected
    }
    
    public interface ICommandExecutor
    {
        Task Execute(ICommand command, IMessageMetadata metadata = null,CommandConfirmationMode confirm = CommandConfirmationMode.Projected);
        ICommandWaiter Prepare<T>(T cmd, IMessageMetadata metadata = null) where T : ICommand;
    }
}