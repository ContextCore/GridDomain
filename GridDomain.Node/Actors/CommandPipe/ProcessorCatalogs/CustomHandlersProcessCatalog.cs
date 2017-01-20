using System.Collections.Generic;
using GridDomain.EventSourcing;

namespace GridDomain.Node.Actors.CommandPipe.ProcessorCatalogs
{
    internal class CustomHandlersProcessCatalog : ProcessorListCatalog<object>, ICustomHandlersProcessorCatalog
    {
        public IReadOnlyCollection<Processor> GetHandlerProcessor(object evt)
        {
            return GetProcessor(evt);
        }
    }
}