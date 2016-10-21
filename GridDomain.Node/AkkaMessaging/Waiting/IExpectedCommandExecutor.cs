using System.Threading.Tasks;
using GridDomain.CQRS;

namespace GridDomain.Node.AkkaMessaging.Waiting
{
    public interface IExpectedCommandExecutor
    {
        Task<IWaitResults> Execute<T>(T command, bool failOnFaults = false) where T : ICommand;
    }
}