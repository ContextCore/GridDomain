using System.Collections.Generic;
using GridDomain.CQRS;
using GridDomain.EventSourcing;

namespace GridDomain.Node.Actors.CommandPipe.ProcessorCatalogs
{
    public interface ISagaProcessorCatalog
    {
        /// <summary>
        ///Returns empty list if no processor was found
        /// </summary>
        /// <param name="evt"></param>
        /// <returns></returns>
        IReadOnlyCollection<Processor> GetSagaProcessor(object evt);
    }
}