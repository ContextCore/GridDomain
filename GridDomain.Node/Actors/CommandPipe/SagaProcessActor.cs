using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Akka.Actor;
using GridDomain.Common;
using GridDomain.CQRS;
using GridDomain.EventSourcing;
using GridDomain.Node.Actors.CommandPipe.ProcessorCatalogs;

namespace GridDomain.Node.Actors.CommandPipe
{
    /// <summary>
    /// Synhronize sagas processing for produced domain events
    /// If message process policy is set to synchronized, will process such events one after one
    /// Will process all other messages in parallel
    /// </summary>
    public class SagaProcessActor : ReceiveActor
    {
        private readonly ISagaProcessorCatalog _catalog;
        private IActorRef _commandExecutionActor;
        public const string SagaProcessActorRegistrationName = nameof(SagaProcessActorRegistrationName);

        public SagaProcessActor(ISagaProcessorCatalog catalog)
        {
            _catalog = catalog;
        
            Receive<Initialize>(i =>
            {
                _commandExecutionActor = i.CommandExecutorActor;
                Sender.Tell(Initialized.Instance);
            });
            //results of one command execution
            Receive<AllHandlersCompleted>(c =>
            {
                c.DomainEvents.Select(e => ProcessSagas(new MessageMetadataEnvelop<DomainEvent>(e, c.Metadata)))
                         .ToChain()
                         .ContinueWith(t => new SagasProcessComplete(t.Result?.ToArray(), t.Exception, c.Metadata))
                         .PipeTo(Self);
            });
            Receive<IMessageMetadataEnvelop<IFault>>(c =>
            {
                ProcessSagas(c).ContinueWith(t => new SagasProcessComplete(t.Result?.ToArray(), t.Exception, c.Metadata))
                               .PipeTo(Self);
            });

            Receive<SagasProcessComplete>(m =>
            {
                foreach (var command in m.ProducedCommands)
                    _commandExecutionActor.Tell(new MessageMetadataEnvelop<ICommand>(command, m.Metadata));
            });
        }

        private Task<IEnumerable<ICommand>> ProcessSagas(IMessageMetadataEnvelop messageMetadataEnvelop)
        {
            IReadOnlyCollection<Processor> eventProcessors = _catalog.GetSagaProcessor(messageMetadataEnvelop.Message);
            if(!eventProcessors.Any())
                 return Task.FromResult(Enumerable.Empty<ICommand>());

            return Task.WhenAll(eventProcessors.Select(e => e.ActorRef.Ask<SagaTransited>(messageMetadataEnvelop)))
                       .ContinueWith(t => t.Result.SelectMany(c => c.ProducedCommands));
        }
    }
}