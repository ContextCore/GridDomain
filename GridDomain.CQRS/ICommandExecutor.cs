using System.Threading.Tasks;
using GridDomain.Common;

namespace GridDomain.CQRS
{
    public interface ICommandExecutor
    {
        Task Execute(ICommand command, IMessageMetadata metadata = null);
        ICommandWaiter Prepare<T>(T cmd, IMessageMetadata metadata = null) where T : ICommand;
    }
}