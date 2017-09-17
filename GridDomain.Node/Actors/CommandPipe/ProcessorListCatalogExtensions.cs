using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GridDomain.Common;
using GridDomain.Node.Actors.CommandPipe.MessageProcessors;

namespace GridDomain.Node.Actors.CommandPipe {
    public static class ProcessorListCatalogExtensions
    {
        public static async Task ProcessMessage(this IProcessorListCatalog processorListCatalog, IMessageMetadataEnvelop envelop)
        {
            var processors = processorListCatalog.Get(envelop.Message);
            Task finalTask = Task.CompletedTask;
            foreach (var p in processors)
                await p.Process(envelop, ref finalTask);
            await finalTask;
        }

        public static async Task<T[]> ProcessMessage<T>(this IProcessorListCatalog<T> processorListCatalog, IMessageMetadataEnvelop envelop)
        {
            var processors = processorListCatalog.Get(envelop.Message);
            Task finalTask = Task.CompletedTask;
            var results = processors.Select(p => p.Process(envelop, ref finalTask)).ToArray();
            await finalTask;
            return results.Select(r => r.Result).ToArray();
        }
    }
}