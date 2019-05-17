using System.Collections.Generic;

namespace GridDomain.Aggregates
{
    public interface ICommandsResultAdapter
    {
        object Adapt(object command, IReadOnlyCollection<IDomainEvent> result);
    }
}