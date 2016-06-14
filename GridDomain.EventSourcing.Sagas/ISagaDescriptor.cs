using System;
using System.Collections.Generic;

namespace GridDomain.EventSourcing.Sagas
{
    public interface ISagaDescriptor
    {
        //TODO: enforce check all messages are DomainEvents
        IReadOnlyCollection<Type> AcceptMessages { get; }
        IReadOnlyCollection<Type> ProduceCommands { get; }
        Type StartMessage { get; } 
        Type StateType { get; }
        Type SagaType { get; }
    }
}