using GridDomain.Common;
using GridDomain.CQRS;

namespace GridDomain.Node.Actors.CommandPipe.Messages
{
    public class ProcessTransitComplete
    {
        public ProcessTransitComplete(IMessageMetadataEnvelop initialMessage, IMessageMetadataEnvelop<ICommand>[] producedCommands)
        {
            InitialMessage = initialMessage;
            ProducedCommands = producedCommands;
        }

        public IMessageMetadataEnvelop InitialMessage { get; }
        public IMessageMetadataEnvelop<ICommand>[] ProducedCommands { get; }

        public static ProcessTransitComplete NoResults { get; } = new ProcessTransitComplete(null,new IMessageMetadataEnvelop<ICommand>[] { });
    }
}