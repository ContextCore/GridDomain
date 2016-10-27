using System;
using System.Collections.Generic;
using CommonDomain;

namespace GridDomain.EventSourcing.Sagas
{
    public interface ISagaDescriptor
    {
        //TODO: enforce check all messages are DomainEvents
        IReadOnlyCollection<MessageBind> AcceptMessages { get; }
        IReadOnlyCollection<Type> ProduceCommands { get; }
        IReadOnlyCollection<Type> StartMessages { get; } 
        Type StateType { get; }
        Type SagaType { get; }
        Type StateMachineType { get; }
    }
}