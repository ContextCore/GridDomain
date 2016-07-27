using System.Collections.Generic;
using CommonDomain;
using NEventStore;

namespace GridDomain.EventSourcing.VersionedTypeSerialization
{
    public interface IDomainEventAdapter
    {
        AdapterDescriptor Descriptor { get; }
        IEnumerable<object> Convert(IAggregate aggregate, object evt);
    }
}