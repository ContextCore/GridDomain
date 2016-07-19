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
            //router.RegisterHandler<DomainEvent,DefaultMessageLoggerHandler>(e => e.SourceId);
            //router.RegisterHandler<ICommand,DefaultMessageLoggerHandler>(e => e.Id);
            //router.RegisterHandler<ICommandFault,DefaultMessageLoggerHandler>(e => e.Id);

            router.Route<ICommandFault>().ToHandler<DefaultMessageLoggerHandler>().Register();
            router.Route<ICommand>().ToHandler<DefaultMessageLoggerHandler>().Register();
            router.Route<DomainEvent>().ToHandler<DefaultMessageLoggerHandler>().Register();

            //router.RegisterHandler<object,DefaultMessageLoggerHandler>(e => new Gui);
        }
    }
}