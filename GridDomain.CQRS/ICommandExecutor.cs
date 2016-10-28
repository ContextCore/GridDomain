using System.Linq.Expressions;
using System.Threading.Tasks;

namespace GridDomain.CQRS
{
    public interface ICommandExecutor
    {
        void Execute(params ICommand[] commands);
        Task<object> Execute(CommandPlan plan);
        Task<T> Execute<T>(CommandPlan<T> plan);
    }
}