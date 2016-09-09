using GridDomain.CQRS;
using GridDomain.EventSourcing;
using GridDomain.Logging;

namespace GridDomain.Node
{
    public class DefaultMessageLoggerHandler : IHandler<DomainEvent>,
                                            IHandler<ICommand>,
                                            IHandler<ICommandFault>
    {
        private static readonly ISoloLogger Log = LogManager.GetLogger(new DefaultLoggerFactory(new GridDomainInternalLoggerConfiguration())).ForContext("GridInternal", true);
        private void Handle(object msg)
        {
            Log.Info("got message from transpot: {@msg}", msg);
        }

        public void Handle(DomainEvent msg)
        {
            Handle((object)msg);
        }

        public void Handle(ICommand msg)
        {
            Handle((object)msg);
        }

        public void Handle(ICommandFault msg)
        {
            Handle((object)msg);
        }
    }
}