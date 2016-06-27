using CommonDomain.Core;
using GridDomain.EventSourcing.Sagas;

namespace GridDomain.CQRS.Messaging.MessageRouting
{
    public interface IMessagesRouter
    {
        IRouteBuilder<TMessage> Route<TMessage>();

        void RegisterAggregate<TAggregate, TCommandHandler>()
            where TCommandHandler : AggregateCommandsHandler<TAggregate>, new()
            where TAggregate : AggregateBase;

        void RegisterAggregate(IAggregateCommandsHandlerDesriptor descriptor);

        void RegisterSaga(ISagaDescriptor sagaDescriptor);
    }
}