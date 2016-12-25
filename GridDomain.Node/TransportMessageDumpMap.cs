using System.Threading.Tasks;
using GridDomain.CQRS;
using GridDomain.CQRS.Messaging;
using GridDomain.CQRS.Messaging.MessageRouting;
using GridDomain.EventSourcing;

namespace GridDomain.Node
{
    public class TransportMessageDumpMap : IMessageRouteMap
    {
        public async Task Register(IMessagesRouter router)
        {
            await router.RegisterHandler<DomainEvent, DefaultMessageLoggerHandler>();
            await router.RegisterHandler<ICommand,DefaultMessageLoggerHandler>();
            await router.RegisterHandler<IFault, DefaultMessageLoggerHandler>();
        }
    }
}