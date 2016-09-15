using GridDomain.CQRS;
using GridDomain.EventSourcing;
using GridDomain.Logging;

namespace GridDomain.Node
{
    public class DefaultMessageLoggerHandler : IHandler<DomainEvent>,
                                               IHandler<ICommand>,
                                               IHandler<IFault>
    {
        private static readonly ISoloLogger Log = LogManager.GetLogger().ForContext("GridInternal", true);
        private void Handle(object msg)
        {
            Log.Trace("got message from transpot: {@msg}", msg);
        }

        public void Handle(DomainEvent msg)
        {
            Handle((object)msg);
        }

        public void Handle(ICommand msg)
        {
            Handle((object)msg);
        }

        public void Handle(IFault msg)
        {
            Handle((object)msg);
        }
    }
}