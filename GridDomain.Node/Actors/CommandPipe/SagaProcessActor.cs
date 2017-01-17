using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Akka.Actor;
using GridDomain.Common;
using GridDomain.CQRS;
using GridDomain.EventSourcing;

namespace GridDomain.Node.Actors
{
    /// <summary>
    /// Synhronize sagas processing for produced domain events
    /// If message process policy is set to synchronized, will process such events one after one
    /// Will process all other messages in parallel
    /// </summary>
    public class SagaProcessActor : ReceiveActor
    {
        private readonly ISagaProcessorCatalog _catalog;
        private readonly IActorRef _commandExecutionActor;

        public SagaProcessActor(ISagaProcessorCatalog catalog, IActorRef commandExecutionActor)
        {
            _catalog = catalog;
            _commandExecutionActor = commandExecutionActor;
        }

        public SagaProcessActor()
        {
            Receive<IMessageMetadataEnvelop<DomainEvent[]>>(c =>
            {
                var eventsToProcess = c.Message;
                var sender = Sender;
                eventsToProcess.Select(e => ProcessSagas(e, c.Metadata))
                               .ToChain()
                               .ContinueWith(t => new SagasProcessComplete(t.Result.ToArray(), c.Metadata))
                               .PipeTo(Self,sender);
            });

            Receive<SagasProcessComplete>(m =>
            {
                Sender.Tell(m);
                foreach (var command in m.ProducedCommands)
                    _commandExecutionActor.Tell(MessageMetadataEnvelop.NewGeneric(command, m.Metadata));
            });
        }

        private Task<IEnumerable<ICommand>> ProcessSagas(DomainEvent evt, IMessageMetadata metadata)
        {
            IReadOnlyCollection<Processor> eventProcessors = _catalog.GetSagaProcessor(evt);
            if(!eventProcessors.Any())
                 return Task.FromResult(Enumerable.Empty<ICommand>());

            var messageMetadataEnvelop = MessageMetadataEnvelop.NewGeneric(evt, metadata);

            var allAsyncTask = new List<Task<IEnumerable<ICommand>>>();

            Task<IEnumerable<ICommand>> resultTask = null;

            foreach (var p in eventProcessors)
            {
                Task<IEnumerable<ICommand>> sagaProcessTask = p.ActorRef.Ask<SagasProcessComplete>(messageMetadataEnvelop)
                                                                        .ContinueWith(t => (IEnumerable<ICommand>)t.Result.ProducedCommands);
                if (p.Policy.IsSynchronious)
                {
                    resultTask = resultTask?.ContinueWith(t => sagaProcessTask.Result.Union(t.Result)) ?? sagaProcessTask;
                }
                else
                {
                    allAsyncTask.Add(sagaProcessTask);
                }
            }
            allAsyncTask.Add(resultTask);

            return Task.WhenAll(allAsyncTask)
                       .ContinueWith(t => t.Result.SelectMany(c => c));
        }

    }
}