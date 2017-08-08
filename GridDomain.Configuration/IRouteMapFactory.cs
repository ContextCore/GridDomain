using GridDomain.Configuration.MessageRouting;

namespace GridDomain.Configuration {
    public interface IRouteMapFactory
    {
        IMessageRouteMap CreateRouteMap();
    }
}