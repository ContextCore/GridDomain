using System;
using GridDomain.Common;
using GridDomain.CQRS;

namespace GridDomain.Node.Actors.CommandPipe
{
    public class SagasProcessComplete
    {
        public SagasProcessComplete(ICommand[] producedCommands, Exception fault, IMessageMetadata metadata)
        {
            ProducedCommands = producedCommands;
            Fault = fault;
            Metadata = metadata;
        }

        public ICommand[] ProducedCommands { get; }
        public Exception Fault { get; }
        public IMessageMetadata Metadata { get; }
    }
}