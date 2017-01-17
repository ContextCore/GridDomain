using System.Collections.Generic;
using GridDomain.EventSourcing;

namespace GridDomain.Node.Actors.CommandPipe.ProcessorCatalogs
{
    public interface ICustomHandlersProcessorCatalog
    {
        /// <summary>
        /// Returns empty list if no processor was found
        /// </summary>
        /// <param name="evt"></param>
        /// <returns></returns>
        IReadOnlyCollection<Processor> GetHandlerProcessor(DomainEvent evt);
    }
}