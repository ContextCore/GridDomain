using System;
using GridDomain.Common;
using GridDomain.CQRS;

namespace GridDomain.Node.Actors.CommandPipe
{
    public class SagaTransited
    {
        public ICommand[] ProducedCommands { get; }
        public IMessageMetadata Metadata { get; }
        public Exception Error { get; }

        public SagaTransited(ICommand[] producedCommands, IMessageMetadata metadata, Exception error = null)
        {
            ProducedCommands = producedCommands;
            Metadata = metadata;
            Error = error;
        }

        public static SagaTransited CreateError(Exception ex, IMessageMetadata metadata = null)
        {
            return new SagaTransited(null,metadata,ex);
        }
    }
}