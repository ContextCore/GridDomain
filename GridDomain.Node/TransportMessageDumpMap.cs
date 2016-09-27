using GridDomain.CQRS;
using GridDomain.CQRS.Messaging;
using GridDomain.CQRS.Messaging.MessageRouting;
using GridDomain.EventSourcing;

namespace GridDomain.Node
{
    public class TransportMessageDumpMap : IMessageRouteMap
    {
        public void Register(IMessagesRouter router)
        {
            router.RegisterHandler<DomainEvent,DefaultMessageLoggerHandler>();
            router.RegisterHandler<ICommand,DefaultMessageLoggerHandler>();
            router.RegisterHandler<IFault, DefaultMessageLoggerHandler>();
        }
    }
}