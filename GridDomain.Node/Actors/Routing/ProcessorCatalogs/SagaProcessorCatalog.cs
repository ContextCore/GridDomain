using System.Collections.Generic;
using GridDomain.EventSourcing;

namespace GridDomain.Node.Actors
{
    class SagaProcessorCatalog : ProcessorListCatalog<DomainEvent>, ISagaProcessorCatalog
    {
        public IReadOnlyCollection<Processor> GetSagaProcessor(DomainEvent evt)
        {
            return GetProcessor(evt);
        }
    }
}