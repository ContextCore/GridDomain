using System.Threading.Tasks;
using GridDomain.EventSourcing;
using GridDomain.EventSourcing.Sagas;

namespace GridDomain.CQRS.Messaging.MessageRouting
{
    public interface IMessagesRouter
    {
        Task RegisterAggregate(IAggregateCommandsHandlerDescriptor descriptor);

        Task RegisterSaga(ISagaDescriptor sagaDescriptor, string name = null);

        Task RegisterHandler<TMessage, THandler>(string correlationField) where THandler : IHandler<TMessage>
            where TMessage : DomainEvent;
    }
}