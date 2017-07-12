using GridDomain.Common;
using GridDomain.CQRS;
using GridDomain.EventSourcing.Sagas.InstanceSagas;

namespace GridDomain.Node.Actors.CommandPipe.Messages
{
    public class SagaTransited : MessageMetadataEnvelop<ICommand[]>,
                                 ISagaTransitCompleted
    {
        public SagaTransited(ICommand[] producedCommands,
                             IMessageMetadata metadata,
                             ProcessEntry sagaProcesEntry, 
                             ISagaState newSagaState) : base(producedCommands, metadata)
        {
            SagaProcessEntry = sagaProcesEntry;
            NewSagaState = newSagaState;
        }

        public ICommand[] ProducedCommands => Message;
        public ISagaState NewSagaState { get; }
        public ProcessEntry SagaProcessEntry { get; }
    }
}