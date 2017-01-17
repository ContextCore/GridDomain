using GridDomain.Common;
using GridDomain.CQRS;

namespace GridDomain.Node.Actors
{
    public class SagasProcessComplete
    {
        public ICommand[] ProducedCommands { get;}
        public IMessageMetadata Metadata { get;}

        public SagasProcessComplete(ICommand[] producedCommands, IMessageMetadata metadata)
        {
            ProducedCommands = producedCommands;
            Metadata = metadata;
        }
    }
}