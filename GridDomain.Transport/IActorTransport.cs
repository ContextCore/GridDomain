using GridDomain.Common;

namespace GridDomain.Transport
{
    public interface IActorTransport : IPublisher,
                                       IActorSubscriber {}
}