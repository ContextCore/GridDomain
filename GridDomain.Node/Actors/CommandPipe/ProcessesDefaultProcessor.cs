using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Akka.Util.Internal;
using GridDomain.Common;
using GridDomain.CQRS;
using GridDomain.EventSourcing;
using GridDomain.Node.Actors.CommandPipe.MessageProcessors;
using GridDomain.Node.Actors.CommandPipe.Messages;
using GridDomain.Node.Actors.Logging;

namespace GridDomain.Node.Actors.CommandPipe
{
    public class ProcessesDefaultProcessor: ICompositeMessageProcessor<ProcessesTransitComplete, IProcessCompleted>
    {
       public void Add<TMessage>(IMessageProcessor<IProcessCompleted> processor)
       {
           _catalog.Add<TMessage>(processor);
       }
        public void Add(Type type,IMessageProcessor<IProcessCompleted> processor)
        {
            _catalog.Add(type, processor);
        }

        private readonly SeveralHandlersTypeCatalog<IMessageProcessor<IProcessCompleted>> _catalog;

        public ProcessesDefaultProcessor()
        {
            _catalog = new SeveralHandlersTypeCatalog<IMessageProcessor<IProcessCompleted>>();
        }

        public async Task<ProcessesTransitComplete> Process(IMessageMetadataEnvelop messageMetadataEnvelop)
        {
            var processors = _catalog.Get(messageMetadataEnvelop.Message);
            var results = new List<object>();
            foreach (var p in processors)
            {
                var processTransitionResult = await p.Process(messageMetadataEnvelop);
                results.Add(processTransitionResult);
            }

            var processTransited = results.OfType<ProcessTransited>().ToArray();

            if(!processTransited.Any())
                return ProcessesTransitComplete.NoResults;
            
            return new ProcessesTransitComplete(messageMetadataEnvelop,processTransited.SelectMany(p =>p.ProducedCommands).ToArray());
        }

        //private static IEnumerable<IMessageMetadataEnvelop<ICommand>> CreateCommandEnvelops(IEnumerable<ProcessTransited> messages)
        //{
        //    return
        //        messages.SelectMany(msg => msg.ProducedCommands
        //                                      .Select(c => new MessageMetadataEnvelop<ICommand>(c,
        //                                                                                        msg.Metadata.CreateChild(c.Id, msg.ProcessTransitEntry))));
        //}

        Task IMessageProcessor.Process(IMessageMetadataEnvelop message)
        {
            return Process(message);
        }
    }
}