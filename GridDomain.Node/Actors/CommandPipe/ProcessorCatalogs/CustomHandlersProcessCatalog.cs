using System.Collections.Generic;
using GridDomain.EventSourcing;

namespace GridDomain.Node.Actors.CommandPipe.ProcessorCatalogs
{
    internal class CustomHandlersProcessCatalog : ProcessorListCatalog<DomainEvent>, ICustomHandlersProcessorCatalog
    {
        public IReadOnlyCollection<Processor> GetHandlerProcessor(DomainEvent evt)
        {
            return GetProcessor(evt);
        }
    }
}