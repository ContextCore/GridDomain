using System;

namespace GridDomain.Common
{
    //TODO: may be add message type to not get it always from object? 
    public interface IMessageMetadata
    {
        string CasuationId { get; }
        string CorrelationId { get; }
        string MessageId { get; }
        ProcessHistory History { get; }
    }
}