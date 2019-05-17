using System.Collections.Generic;

namespace GridDomain.Aggregates.Abstractions
{
    public interface ICommandsResultAdapter
    {
        object Adapt(object command, IReadOnlyCollection<IDomainEvent> result);
    }
}