using System.Threading.Tasks;

namespace GridDomain.CQRS
{
    public interface IExpectedCommandExecutor
    {
         Task<IWaitResults> Execute<T>(params T[] command) where T : ICommand;
    }
}