using GridDomain.Configuration.MessageRouting;
using Serilog;

namespace GridDomain.Configuration {
    public interface IMessageProcessContext
    {
        IPublisher Publisher { get; }
        ILogger Log { get; }
    }
}