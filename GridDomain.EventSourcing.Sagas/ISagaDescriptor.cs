using System;
using System.Collections.Generic;

namespace GridDomain.EventSourcing.Sagas
{
    public interface ISagaDescriptor
    {
        //TODO: enforce check all messages are DomainEvents
        IReadOnlyCollection<Type> AcceptMessages { get; }
        IReadOnlyCollection<Type> ProduceCommands { get; }
        IReadOnlyCollection<Type> StartMessages { get; } 
        Type StateType { get; }
        Type SagaType { get; }
    }
}