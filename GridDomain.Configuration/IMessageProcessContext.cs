using GridDomain.CQRS.Messaging;
using Serilog;

namespace GridDomain.Configuration {
    public interface IMessageProcessContext
    {
        IPublisher Publisher { get; }
        ILogger Log { get; }
    }
}