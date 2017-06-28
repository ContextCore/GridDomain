using GridDomain.CQRS.Messaging;

namespace GridDomain.Node.Configuration.Composition {
    public interface IRouteMapFactory
    {
        IMessageRouteMap CreateRouteMap();
    }
}