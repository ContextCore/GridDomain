using System;
using System.Collections.Generic;

namespace GridDomain.ProcessManagers.DomainBind
{
    public interface IProcessManagerDescriptor
    {
        //TODO: enforce check all messages are DomainEvents
        IReadOnlyCollection<MessageBind> AcceptMessages { get; }
        IReadOnlyCollection<Type> ProduceCommands { get; }
        IReadOnlyCollection<Type> StartMessages { get; }
        Type StateType { get; }
        Type ProcessType { get; }
    }
}