using System.Collections.Generic;
using GridDomain.Common;
using GridDomain.CQRS;
using GridDomain.ProcessManagers;

namespace GridDomain.Node.Actors.CommandPipe.Messages
{
    public class ProcessTransited : MessageMetadataEnvelop<IReadOnlyCollection<ICommand>>,
                                 IProcessCompleted
    {
        public ProcessTransited(IReadOnlyCollection<ICommand> producedCommands,
                                IMessageMetadata metadata,
                                ProcessEntry procesTransitEntry, 
                                IProcessState newProcessState) : base(producedCommands, metadata)
        {
            ProcessTransitEntry = procesTransitEntry;
            NewProcessState = newProcessState;
        }

        public IReadOnlyCollection<ICommand> ProducedCommands => Message;
        public IProcessState NewProcessState { get; }
        public ProcessEntry ProcessTransitEntry { get; }
    }
}