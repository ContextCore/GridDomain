using GridDomain.CQRS.Messaging;

namespace GridDomain.Node
{
    public interface IActorTransport : IPublisher, IActorSubscriber
    {
    }
}