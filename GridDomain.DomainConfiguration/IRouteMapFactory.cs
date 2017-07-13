using GridDomain.CQRS.Messaging;

namespace GridDomain.Configuration {
    public interface IRouteMapFactory
    {
        IMessageRouteMap CreateRouteMap();
    }
}