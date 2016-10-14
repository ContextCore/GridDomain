using System.Threading.Tasks;
using GridDomain.CQRS;

namespace GridDomain.Node.AkkaMessaging.Waiting
{
    public interface ICommandWaiter : IMessageWaiter
    {
        ICommandWaiter Execute(ICommand command);
        Task<object> WaitFor(CommandPlan p);
    }
}