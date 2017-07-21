using System;
using System.Collections.Generic;

namespace GridDomain.EventSourcing.Aggregates
{
    public interface IAggregateCommandsHandlerDescriptor
    {
        IReadOnlyCollection<Type> RegisteredCommands { get; }
        Type AggregateType { get; }
    }
}