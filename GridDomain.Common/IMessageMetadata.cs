using System;

namespace GridDomain.Common
{
    public interface IMessageMetadata
    {
        Guid MessageId { get; }
        Guid CasuationId { get; }
        Guid CorrelationId { get; }
        ProcessHistory History { get; }
    }
}