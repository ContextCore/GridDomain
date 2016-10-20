using System.Threading.Tasks;
using GridDomain.CQRS;

namespace GridDomain.Node.AkkaMessaging.Waiting
{
    public interface IExpectedCommandExecutor
    {
        Task<IWaitResults> Execute(ICommand command);

       // Task<IWaitResults> Awaiter { get; }
       // ICommandExecutor Executor { get; }
    }
}