using GridDomain.Common;
using GridDomain.CQRS;

namespace GridDomain.Node.Actors.CommandPipe.Messages
{
    public class ProcessesTransitComplete
    {
        public ProcessesTransitComplete(IMessageMetadataEnvelop initialMessage, IMessageMetadataEnvelop<ICommand>[] producedCommands)
        {
            InitialMessage = initialMessage;
            ProducedCommands = producedCommands;
        }

        public IMessageMetadataEnvelop InitialMessage { get; }
        public IMessageMetadataEnvelop<ICommand>[] ProducedCommands { get; }

        public static ProcessesTransitComplete NoResults { get; } = new ProcessesTransitComplete(null,new IMessageMetadataEnvelop<ICommand>[] { });
    }
}