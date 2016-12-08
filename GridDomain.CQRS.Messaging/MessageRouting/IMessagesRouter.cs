using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CommonDomain.Core;
using GridDomain.EventSourcing;
using GridDomain.EventSourcing.Sagas;
using GridDomain.EventSourcing.Sagas.InstanceSagas;

namespace GridDomain.CQRS.Messaging.MessageRouting
{

    

    public interface IMessagesRouter
    {
        [Obsolete("Use RegisterHandler instead")]
        IRouteBuilder<TMessage> Route<TMessage>();

        Task RegisterAggregate<TAggregate, TCommandHandler>()
            where TCommandHandler : AggregateCommandsHandler<TAggregate>, new()
            where TAggregate : AggregateBase;

        Task RegisterAggregate(IAggregateCommandsHandlerDesriptor descriptor);

        Task RegisterSaga(ISagaDescriptor sagaDescriptor, string name = null);

        Task RegisterHandler<TMessage, THandler>(string correlationField) where THandler : IHandler<TMessage>;

        Task RegisterProjectionGroup<T>(T group) where T : IProjectionGroup;
    }

   
}
