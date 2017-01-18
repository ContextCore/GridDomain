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

        public SagaProcessActor(ISagaProcessorCatalog catalog)
        {
            _catalog = catalog;

            Receive<Initialize>(i =>
            {
                _commandExecutionActor = i.CommandExecutorActor;
                Sender.Tell(Initialized.Instance);
                Become(Working);
            });
        }

        private void Working()
        {
            Receive<IMessageMetadataEnvelop<DomainEvent[]>>(c =>
            {
                c.Message.Select(e => ProcessSagas(e, c.Metadata))
                    .ToChain()
                    .ContinueWith(t => new SagasProcessComplete(t.Result.ToArray(), c.Metadata))
                    .PipeTo(Self, Sender);
            });

            Receive<SagasProcessComplete>(m =>
            {
                Sender.Tell(m);
                foreach (var command in m.ProducedCommands)
                    _commandExecutionActor.Tell(new MessageMetadataEnvelop<ICommand>(command, m.Metadata));
            });
        }

        private Task<IEnumerable<ICommand>> ProcessSagas(DomainEvent evt, IMessageMetadata metadata)
        {
            IReadOnlyCollection<Processor> eventProcessors = _catalog.GetSagaProcessor(evt);
            if(!eventProcessors.Any())
                 return Task.FromResult(Enumerable.Empty<ICommand>());

            var messageMetadataEnvelop = new MessageMetadataEnvelop<DomainEvent>(evt, metadata);

            return Task.WhenAll(eventProcessors.Select(e => e.ActorRef.Ask<SagaProcessCompleted>(messageMetadataEnvelop)
                                               .ContinueWith(t => (IEnumerable<ICommand>)t.Result.ProducedCommands)))
                       .ContinueWith(t => t.Result.SelectMany(c => c));
        }

    }

    public class Initialize
    {
        public Initialize(IActorRef commandExecutorActor)
        {
            CommandExecutorActor = commandExecutorActor;
        }

        public IActorRef CommandExecutorActor { get; }
    }

    public class Initialized
    {
        private Initialized()
        {
        }

        public static Initialized Instance { get; } = new Initialized();
    }
}