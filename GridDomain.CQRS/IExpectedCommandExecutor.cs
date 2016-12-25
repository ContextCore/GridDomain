using System.Threading.Tasks;
using GridDomain.Common;

namespace GridDomain.CQRS
{
    public interface IExpectedCommandExecutor
    {
        Task<IWaitResults> Execute(params ICommand[] command);
         Task<IWaitResults> Execute<T>(T command, IMessageMetadata metadata) where T : ICommand;
    }
}