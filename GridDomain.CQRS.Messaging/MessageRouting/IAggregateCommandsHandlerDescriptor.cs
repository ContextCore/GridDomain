using System;
using System.Collections.Generic;

namespace GridDomain.CQRS.Messaging.MessageRouting
{
    public interface IAggregateCommandsHandlerDescriptor
    {
        IReadOnlyCollection<Type> RegisteredCommands { get; }
        Type AggregateType { get; }
    }
}