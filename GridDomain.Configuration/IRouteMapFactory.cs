using GridDomain.Routing;

namespace GridDomain.Configuration {
    public interface IRouteMapFactory
    {
        IMessageRouteMap CreateRouteMap();
    }
}