using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GridDomain.Common;

namespace GridDomain.Node.Actors.CommandPipe.MessageProcessors
{
   

    public class HandlersDefaultProcessor : ICompositeMessageProcessor
    {
        private readonly SeveralHandlersTypeCatalog<IMessageProcessor> _processorListCatalog;

        public HandlersDefaultProcessor()
        {
            _processorListCatalog = new SeveralHandlersTypeCatalog<IMessageProcessor>();
        }

        public void Add(Type messageType, IMessageProcessor messageProcessor)
        {
            _processorListCatalog.Add(messageType, messageProcessor);
        }

        public async Task Process(IMessageMetadataEnvelop envelop)
        {
            foreach(var p in _processorListCatalog.Get(envelop.Message))
                await p.Process(envelop);
        }
    }



}