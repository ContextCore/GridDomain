using System.Threading.Tasks;

namespace GridDomain.Node.AkkaMessaging.Waiting
{
    public interface IMessageWaiterFactory
    {
        IMessageWaiter<Task<IWaitResults>>  NewWaiter();
        IMessageWaiter<IExpectedCommandExecutor>  NewCommandWaiter();
    }
}