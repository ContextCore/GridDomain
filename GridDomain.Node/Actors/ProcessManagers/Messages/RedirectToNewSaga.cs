using System;
using GridDomain.Common;
using GridDomain.Node.Actors.CommandPipe.Messages;

namespace GridDomain.Node.Actors.ProcessManagers.Messages
{
    public class ProcessRedirect :  IProcessCompleted
    {
        public IMessageMetadataEnvelop MessageToRedirect { get; }
        public Guid ProcessId { get; }

        public ProcessRedirect(Guid processId, IMessageMetadataEnvelop messageToRedirect)
        {
            ProcessId = processId;
            MessageToRedirect = messageToRedirect;
        }
    }
}