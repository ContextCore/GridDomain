using System.Threading.Tasks;

namespace GridDomain.CQRS
{
    public interface ICommandExecutor
    {
        void Execute(params ICommand[] commands);

        Task<object> Execute(CommandPlan plan);
    }
}