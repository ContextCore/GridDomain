using GridDomain.Common;
using GridDomain.CQRS;

namespace GridDomain.Node.Actors.CommandPipe.Messages
{
    public class ProcessFault : MessageMetadataEnvelop<IFault>,
                                    IProcessCompleted
    {
        public ProcessFault(IFault fault, IMessageMetadata metadata) : base(fault, metadata) {}
    }
}