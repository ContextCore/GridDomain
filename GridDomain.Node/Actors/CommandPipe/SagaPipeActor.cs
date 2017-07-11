using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.Util.Internal;
using GridDomain.Common;
using GridDomain.CQRS;
using GridDomain.EventSourcing;
using GridDomain.Node.Actors.CommandPipe.Messages;
using GridDomain.Node.Actors.CommandPipe.Processors;

namespace GridDomain.Node.Actors.CommandPipe
{
    /// <summary>
    ///     Synhronize sagas processing for produced domain events
    ///     If message process policy is set to synchronized, will process such events one after one
    ///     Will process all other messages in parallel
    /// </summary>
    public class SagaPipeActor : ReceiveActor
    {
        public const string SagaProcessActorRegistrationName = nameof(SagaProcessActorRegistrationName);
        private readonly IProcessorListCatalog<ISagaTransitCompleted> _catalog;
        private IActorRef _commandExecutionActor;

        public SagaPipeActor(IProcessorListCatalog<ISagaTransitCompleted> catalog)
        {
            _catalog = catalog;

            Receive<Initialize>(i =>
                                {
                                    _commandExecutionActor = i.CommandExecutorActor;
                                    Sender.Tell(Initialized.Instance);
                                });
            //part of events or fault from command execution
            ReceiveAsync<IMessageMetadataEnvelop>(env => ProcessSagas(env).PipeTo(Self));

            Receive<SagasProcessComplete>(m =>
                                          {
                                              foreach (var envelop in m.ProducedCommands)
                                                  _commandExecutionActor.Tell(envelop);
                                          });
        }
        
        private async Task<SagasProcessComplete> ProcessSagas(IMessageMetadataEnvelop messageMetadataEnvelop)
        {
           var res = await _catalog.ProcessMessage(messageMetadataEnvelop);
           var sagaTransiteds = res.OfType<SagaTransited>().ToArray();
           if(!sagaTransiteds.Any())
               return SagasProcessComplete.NoResults;

           return new SagasProcessComplete(CreateCommandEnvelops(sagaTransiteds).ToArray());
        }

        private static IEnumerable<IMessageMetadataEnvelop<ICommand>> CreateCommandEnvelops(IEnumerable<SagaTransited> messages)
        {
            return
                messages.SelectMany(msg => msg.ProducedCommands
                                              .Select(c => new MessageMetadataEnvelop<ICommand>(c,
                                                                                                msg.Metadata.CreateChild(c.Id, msg.SagaProcessEntry))));
        }
    }
}