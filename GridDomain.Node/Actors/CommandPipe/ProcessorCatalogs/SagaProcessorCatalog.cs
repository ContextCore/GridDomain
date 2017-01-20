using System.Collections.Generic;
using GridDomain.EventSourcing;

namespace GridDomain.Node.Actors.CommandPipe.ProcessorCatalogs
{
    class SagaProcessorCatalog : ProcessorListCatalog<object>, ISagaProcessorCatalog
    {
        public IReadOnlyCollection<Processor> GetSagaProcessor(object evt)
        {
            return GetProcessor(evt);
        }
    }
}