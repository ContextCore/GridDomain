using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Akka.Actor;
using GridDomain.Common;
using GridDomain.EventSourcing;
using GridDomain.Node.Actors.CommandPipe.ProcessorCatalogs;

namespace GridDomain.Node.Actors.CommandPipe
{
    /// <summary>
    /// Synhronize message handlers work for produced domain events
    /// If message process policy is set to synchronized, will process such events one after one
    /// Will process all other messages in parallel
    /// </summary>
    public class HandlersProcessActor : ReceiveActor
    {
        public const string CustomHandlersProcessActorRegistrationName = "CustomHandlersProcessActor";

        private readonly ICustomHandlersProcessorCatalog _handlersCatalog;
        private readonly IActorRef _sagasProcessActor;

        public HandlersProcessActor(ICustomHandlersProcessorCatalog handlersCatalog, IActorRef sagasProcessActor)
        {
            _sagasProcessActor = sagasProcessActor;
            _handlersCatalog = handlersCatalog;
    
            Receive<IMessageMetadataEnvelop<DomainEvent[]>>(envelop =>
            {
                var eventsToProcess = envelop.Message;
                var sender = Sender;
                eventsToProcess.Select(e => ProcessMessageHandlers(e, envelop.Metadata))
                               .ToChain()
                               .ContinueWith(t => new CustomHandlersProcessCompleted(envelop.Metadata, envelop.Message))
                               .PipeTo(Self, sender);
            });

            Receive<CustomHandlersProcessCompleted>(m =>
            {
                Sender.Tell(m); //notifying aggregate actor
                _sagasProcessActor.Tell(m); //starting sagas processing
            });
        }

        private Task ProcessMessageHandlers(DomainEvent evt, IMessageMetadata metadata)
        {
            IReadOnlyCollection<Processor> eventProcessors = _handlersCatalog.GetHandlerProcessor(evt);
            if(!eventProcessors.Any()) return Task.CompletedTask;

            var messageMetadataEnvelop = new MessageMetadataEnvelop<DomainEvent>(evt, metadata);

            return eventProcessors.Select(p => {
                                         if (p.Policy.IsSynchronious)
                                             return p.ActorRef.Ask<HandlerExecuted>(messageMetadataEnvelop);
                                         
                                         p.ActorRef.Tell(messageMetadataEnvelop);
                                              return Task.CompletedTask;
                                         })
                                  .ToChain();
        }

    }
}