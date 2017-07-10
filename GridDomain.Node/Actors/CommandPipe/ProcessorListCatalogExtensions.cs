using System.Linq;
using System.Threading.Tasks;
using GridDomain.Common;
using GridDomain.Node.Actors.CommandPipe.ProcessorCatalogs;

namespace GridDomain.Node.Actors.CommandPipe {
    public static class ProcessorListCatalogExtensions
    {
        public static Task ProcessMessage(this IProcessorListCatalog processorListCatalog, IMessageMetadataEnvelop envelop)
        {
            var processors = processorListCatalog.Get(envelop.Message);
            return processors?.Aggregate<IMessageProcessor, Task>(null,
                                                          (workInProgress, nextProcessor) => nextProcessor.Process(envelop, workInProgress))
                   ?? Task.CompletedTask;
        }
    }
}