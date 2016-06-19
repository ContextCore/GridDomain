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
        Type StartMessage { get; } 
        Type StateType { get; }
        Type SagaType { get; }
    }
}