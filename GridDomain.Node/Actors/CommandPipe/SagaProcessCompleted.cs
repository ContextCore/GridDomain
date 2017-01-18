using GridDomain.Common;
using GridDomain.CQRS;

namespace GridDomain.Node.Actors.CommandPipe
{
    public class SagaProcessCompleted
    {
        public ICommand[] ProducedCommands { get; }
        public IMessageMetadata Metadata { get; }

        public SagaProcessCompleted(ICommand[] producedCommands, IMessageMetadata metadata)
        {
            ProducedCommands = producedCommands;
            Metadata = metadata;
        }
    }
}