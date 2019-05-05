using System.Collections.Generic;
using System.Linq;
using GridDomain.Aggregates;

namespace GridDomain.Node.Tests
{
    class CatCommandsResultResultAdapter : ICommandsResultAdapter
    {
        public object Adapt(object command, IReadOnlyCollection<IDomainEvent> result)
        {
            if (command is Cat.GetNewCatCommand c)
                return result.OfType<Cat.Born>().FirstOrDefault()?.Name;
            return null;
        }
    }
}