using GridDomain.Common;
using GridDomain.Configuration.MessageRouting;
using GridDomain.CQRS;
using Serilog;

namespace GridDomain.Configuration
{
    public interface IMessageProcessContext
    {
        IPublisher Publisher { get; }
        ILogger Log { get; }
    }
}