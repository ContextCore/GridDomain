using GridDomain.Common;
using GridDomain.CQRS;

namespace GridDomain.Node.Actors.CommandPipe.Messages
{
    public class ProcessTransitComplete
    {
        public ProcessTransitComplete(IMessageMetadataEnvelop<ICommand>[] producedCommands)
        {
            ProducedCommands = producedCommands;
        }

        public IMessageMetadataEnvelop<ICommand>[] ProducedCommands { get; }

        public static ProcessTransitComplete NoResults { get; } = new ProcessTransitComplete(new IMessageMetadataEnvelop<ICommand>[] { });
    }
}