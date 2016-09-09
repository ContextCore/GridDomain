using GridDomain.CQRS;
using GridDomain.CQRS.Messaging;
using GridDomain.CQRS.Messaging.MessageRouting;
using GridDomain.EventSourcing;
using GridDomain.Node.MessageDump;

namespace GridDomain.Node
{
    public class TransportMessageDumpMap : IMessageRouteMap
    {
        public void Register(IMessagesRouter router)
        {
            router.RegisterHandler<DomainEvent, MessageDumpHandler>(e => e.SourceId);
            router.RegisterHandler<ICommand, MessageDumpHandler>(e => e.Id);
            router.RegisterHandler<ICommandFault, MessageDumpHandler>(e => e.Id);
        }
    }

    public class TransportMessageLogMap : IMessageRouteMap
    {
        public void Register(IMessagesRouter router)
        {
            router.RegisterHandler<DomainEvent, DefaultMessageLoggerHandler>(e => e.SourceId);
            router.RegisterHandler<ICommand, DefaultMessageLoggerHandler>(e => e.Id);
            router.RegisterHandler<ICommandFault, DefaultMessageLoggerHandler>(e => e.Id);
        }
    }
}