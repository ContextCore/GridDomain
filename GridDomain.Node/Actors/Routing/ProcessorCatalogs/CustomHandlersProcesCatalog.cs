using System.Collections.Generic;
using GridDomain.EventSourcing;

namespace GridDomain.Node.Actors
{
    class CustomHandlersProcesCatalog : ProcessorListCatalog<DomainEvent>, ISagaProcessorCatalog
    {
        public IReadOnlyCollection<Processor> GetSagaProcessor(DomainEvent evt)
        {
            return GetProcessor(evt);
        }
    }
}