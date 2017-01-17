using System;

namespace GridDomain.Common
{
    //TODO: may be add message type to not get it always from object? 
    public interface IMessageMetadata
    {
        Guid MessageId { get; }
        Guid CasuationId { get; }
        Guid CorrelationId { get; }
        ProcessHistory History { get; }
    }
}