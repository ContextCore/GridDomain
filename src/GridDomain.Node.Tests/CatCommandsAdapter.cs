using System.Collections.Generic;
using System.Linq;
using GridDomain.Aggregates;
using GridDomain.Aggregates.Abstractions;

namespace GridDomain.Node.Tests
{
    class CatCommandsResultAdapter : ICommandsResultAdapter
    {
        public object Adapt(object command, IReadOnlyCollection<IDomainEvent> result)
        {
            if (command is Cat.GetNewCatCommand c)
                return result.OfType<Cat.Born>().FirstOrDefault()?.Name;
            return null;
        }
    }
}