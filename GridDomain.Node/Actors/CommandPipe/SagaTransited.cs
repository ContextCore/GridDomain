using GridDomain.Common;
using GridDomain.CQRS;

namespace GridDomain.Node.Actors.CommandPipe
{
    public class SagaTransited
    {
        public ICommand[] ProducedCommands { get; }
        public IMessageMetadata Metadata { get; }

        public SagaTransited(ICommand[] producedCommands, IMessageMetadata metadata)
        {
            ProducedCommands = producedCommands;
            Metadata = metadata;
        }
    }
}