using GridDomain.CQRS.Messaging;
using Serilog;

namespace GridDomain.Node.Configuration.Composition {
    public interface IMessageProcessContext
    {
        IPublisher Publisher { get; }
        ILogger Log { get; }
    }
}