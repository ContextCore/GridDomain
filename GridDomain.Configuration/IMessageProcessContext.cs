using GridDomain.Routing;
using Serilog;

namespace GridDomain.Configuration {
    public interface IMessageProcessContext
    {
        IPublisher Publisher { get; }
        ILogger Log { get; }
    }
}