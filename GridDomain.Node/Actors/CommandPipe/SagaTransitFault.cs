using GridDomain.Common;
using GridDomain.CQRS;

namespace GridDomain.Node.Actors.CommandPipe
{
    public class SagaTransitFault : MessageMetadataEnvelop<IFault>, ISagaTransitCompleted
    {
        public SagaTransitFault(IFault fault, IMessageMetadata metadata) : base(fault, metadata)
        {
        }
    }
}