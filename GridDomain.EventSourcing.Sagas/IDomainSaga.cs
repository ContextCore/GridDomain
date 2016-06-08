using System;
using System.Collections.Generic;
using CommonDomain;

namespace GridDomain.EventSourcing.Sagas
{
    public interface IDomainSaga
    {
        List<object> MessagesToDispatch { get; }
        IAggregate StateAggregate { get; }
        void Handle(object message);
    }

    public interface ISagaDescriptor
    {
        //TODO: enforce check all messages are DomainEvents
        IReadOnlyCollection<Type> AcceptMessages { get; }
        Type StartMessage { get; } 
        Type StateType { get; }

        Type SagaType { get; }
    }
}