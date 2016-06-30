using System;
using System.Collections.Generic;

namespace GridDomain.CQRS.Messaging.MessageRouting
{
    public interface  IAggregateCommandsHandlerDesriptor
    {
        IReadOnlyCollection<AggregateLookupInfo> RegisteredCommands { get; } 
        Type AggregateType { get; }
    }
}