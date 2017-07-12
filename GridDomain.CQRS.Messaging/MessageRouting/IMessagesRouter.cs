using System.Threading.Tasks;
using GridDomain.Common;
using GridDomain.EventSourcing;
using GridDomain.EventSourcing.Sagas;

namespace GridDomain.CQRS.Messaging.MessageRouting
{
    public interface IMessagesRouter
    {
        Task RegisterAggregate(IAggregateCommandsHandlerDescriptor descriptor);

        Task RegisterSaga(ISagaDescriptor sagaDescriptor, string name = null);

        Task RegisterSyncHandler<TMessage, THandler>() where THandler : IHandler<TMessage>
                                                       where TMessage : class, IHaveSagaId, IHaveId;
        Task RegisterFireAndForgetHandler<TMessage, THandler>() where THandler : IHandler<TMessage>
                                                                where TMessage : class, IHaveSagaId, IHaveId;
        Task RegisterParralelHandler<TMessage, THandler>() where THandler : IHandler<TMessage>
                                                           where TMessage : class, IHaveSagaId, IHaveId;
    }
}