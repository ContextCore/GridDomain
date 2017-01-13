using System;
using System.Collections.Generic;

namespace GridDomain.CQRS.Messaging.MessageRouting
{
    public interface IAggregateCommandsHandlerDescriptor
    {
        IReadOnlyCollection<AggregateCommandInfo> RegisteredCommands { get; } 
        Type AggregateType { get; }
    }
}