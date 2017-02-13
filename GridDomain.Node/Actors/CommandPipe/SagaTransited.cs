using System;
using GridDomain.Common;
using GridDomain.CQRS;
using GridDomain.EventSourcing;

namespace GridDomain.Node.Actors.CommandPipe
{
    public class SagaTransited: MessageMetadataEnvelop<ICommand[]>, ISagaTransitCompleted
    {
        public SagaTransited(ICommand[] producedCommands, IMessageMetadata metadata, ProcessEntry sagaProcesEntry,Exception error = null) :base(producedCommands,metadata)
        {
            SagaProcessEntry = sagaProcesEntry;
        }
        public ICommand[] ProducedCommands => Message;
        public ProcessEntry SagaProcessEntry { get; }
    }
}