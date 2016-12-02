using System.Threading.Tasks;

namespace GridDomain.CQRS
{
    public interface IExpectedCommandExecutor
    {
        Task<IWaitResults> Execute(params ICommand[] command);
    }
}