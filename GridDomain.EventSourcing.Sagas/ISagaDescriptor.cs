using System;
using System.Collections.Generic;

namespace GridDomain.EventSourcing.Sagas
{
    //TODO: create descriptor builder from saga to avoid singletons in sagas as empty sagas instances
    public interface ISagaDescriptor
    {
        //TODO: enforce check all messages are DomainEvents
        IReadOnlyCollection<Type> AcceptMessages { get; }
        IReadOnlyCollection<Type> ProduceCommands { get; }
        IReadOnlyCollection<Type> StartMessages { get; } 
        Type StateType { get; }
        Type SagaType { get; }
    }

    //Marker interface to simplify dependency injection into saga actors
    public interface ISagaDescriptor<T> : ISagaDescriptor
    {
        
    }
}