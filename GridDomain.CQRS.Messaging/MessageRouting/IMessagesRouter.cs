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
        Task RegisterAggregate(IAggregateCommandsHandlerDescriptor descriptor);

        Task RegisterSaga(ISagaDescriptor sagaDescriptor, string name = null);

        Task RegisterHandler<TMessage, THandler>(string correlationField) where THandler : IHandler<TMessage>
                                                                          where TMessage : DomainEvent;
    }
}
