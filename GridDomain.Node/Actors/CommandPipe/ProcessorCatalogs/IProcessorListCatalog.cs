using System.Collections.Generic;
using GridDomain.Common;
using GridDomain.EventSourcing;

namespace GridDomain.Node.Actors.CommandPipe.ProcessorCatalogs
{
    public interface IProcessorListCatalog : ICatalog<IReadOnlyCollection<Processor>, object>
    {
    }
}