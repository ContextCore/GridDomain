using System.Collections.Generic;
using GridDomain.EventSourcing;

namespace GridDomain.Node.Actors.CommandPipe.ProcessorCatalogs
{
    class CustomHandlersProcessCatalog : ProcessorListCatalog<DomainEvent>, ISagaProcessorCatalog
    {
        public IReadOnlyCollection<Processor> GetSagaProcessor(DomainEvent evt)
        {
            return GetProcessor(evt);
        }
    }
}