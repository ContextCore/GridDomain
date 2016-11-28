using System;

namespace GridDomain.EventSourcing
{
    public interface IMetadata
    {
        Guid Id { get; }
        Guid CasuationId { get; }
        Guid CorrelationId { get; }
        ProcessHistory History { get; }
    }
}