using GridDomain.Common;
using GridDomain.Configuration.MessageRouting;

namespace GridDomain.Node.Transports
{
    public interface IActorTransport : IPublisher,
                                       IActorSubscriber {}
}