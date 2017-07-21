using GridDomain.Routing;

namespace GridDomain.Node.Transports
{
    public interface IActorTransport : IPublisher,
                                       IActorSubscriber {}
}