using System.Collections.Generic;
using GridDomain.Common;
using GridDomain.CQRS;

namespace GridDomain.Node.Actors.CommandPipe.Messages
{
    public class ProcessesTransitComplete
    {
        public ProcessesTransitComplete(IMessageMetadataEnvelop initialMessage, ICommand[] producedCommands)
        {
            InitialMessage = initialMessage;
            ProducedCommands = producedCommands;
        }

        public IMessageMetadataEnvelop InitialMessage { get; }
        public IReadOnlyCollection<ICommand> ProducedCommands { get; }

        public static ProcessesTransitComplete NoResults { get; } = new ProcessesTransitComplete(null,new ICommand[] { });
    }
}