using GridDomain.CQRS;

namespace GridDomain.Node.AkkaMessaging.Waiting
{
    public interface ICommandWaiter : IMessageWaiter
    {
        IMessageWaiter Execute(ICommand command);


    }
}