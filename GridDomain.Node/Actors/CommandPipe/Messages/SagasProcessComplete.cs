using GridDomain.Common;
using GridDomain.CQRS;

namespace GridDomain.Node.Actors.CommandPipe.Messages
{
    public class SagasProcessComplete
    {
        public SagasProcessComplete(IMessageMetadataEnvelop<ICommand>[] producedCommands)
        {
            ProducedCommands = producedCommands;
        }

        public IMessageMetadataEnvelop<ICommand>[] ProducedCommands { get; }

        public static SagasProcessComplete NoResults { get; } = new SagasProcessComplete(new IMessageMetadataEnvelop<ICommand>[] { });
    }
}