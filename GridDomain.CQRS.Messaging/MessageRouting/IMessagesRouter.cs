using CommonDomain.Core;
using GridDomain.EventSourcing;
using GridDomain.EventSourcing.Sagas;

namespace GridDomain.CQRS.Messaging.MessageRouting
{
    public interface IMessagesRouter
    {
        IRouteBuilder<TMessage> Route<TMessage>();

        void RegisterAggregate<TAggregate, TCommandHandler>()
            where TCommandHandler : AggregateCommandsHandler<TAggregate>, new()
            where TAggregate : AggregateBase;

        //void RegisterSaga<TSaga, TSagaState, TStartMessage>(ISagaDescriptor saga)
                                                //where TSaga : IDomainSaga
                                                //where TSagaState : AggregateBase
                                                //where TStartMessage : DomainEvent;

        void RegisterSaga(ISagaDescriptor sagaDescriptor);
    }
}