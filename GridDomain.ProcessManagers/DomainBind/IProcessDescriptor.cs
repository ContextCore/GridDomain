using System;
using System.Collections.Generic;

namespace GridDomain.ProcessManagers.DomainBind
{
    public interface IProcessDescriptor
    {
        //TODO: enforce check all messages are DomainEvents
        IReadOnlyCollection<MessageBind> AcceptMessages { get; }
        Type StateType { get; }
        Type ProcessType { get; }
    }
}