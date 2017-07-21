using GridDomain.Common;
using GridDomain.CQRS;
using GridDomain.ProcessManagers;

namespace GridDomain.Node.Actors.CommandPipe.Messages
{
    public class ProcessTransited : MessageMetadataEnvelop<ICommand[]>,
                                 IProcessCompleted
    {
        public ProcessTransited(ICommand[] producedCommands,
                             IMessageMetadata metadata,
                             ProcessEntry procesTransitEntry, 
                             IProcessState newProcessState) : base(producedCommands, metadata)
        {
            ProcessTransitEntry = procesTransitEntry;
            NewProcessState = newProcessState;
        }

        public ICommand[] ProducedCommands => Message;
        public IProcessState NewProcessState { get; }
        public ProcessEntry ProcessTransitEntry { get; }
    }
}