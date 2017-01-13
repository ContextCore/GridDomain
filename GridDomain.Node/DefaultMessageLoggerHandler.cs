using System.Threading.Tasks;
using GridDomain.CQRS;
using GridDomain.EventSourcing;
using GridDomain.Logging;

namespace GridDomain.Node
{
    public class DefaultMessageLoggerHandler : IHandler<DomainEvent>,
                                               IHandler<ICommand>,
                                               IHandler<IFault>
    {
        private static readonly ILogger Log = LogManager.GetLogger().ForContext("GridInternal", true);
        private Task Handle(object msg)
        {
            Log.Trace("got message from transpot: {@msg}", msg);
            return Task.CompletedTask;
        }

        public Task Handle(DomainEvent msg)
        {
            return Handle((object)msg);
        }

        public Task Handle(ICommand msg)
        {
            return Handle((object)msg);
        }

        public Task Handle(IFault msg)
        {
            return Handle((object)msg);
        }
    }
}