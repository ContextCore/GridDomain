using System.Threading.Tasks;
using GridDomain.Common;

namespace GridDomain.CQRS
{
    public interface IExpectedCommandExecutor
    {
         Task<IWaitResults> Execute<T>(params T[] command) where T : ICommand;
         Task<IWaitResults> Execute<T>(T command, IMessageMetadata metadata) where T : ICommand;
    }
}