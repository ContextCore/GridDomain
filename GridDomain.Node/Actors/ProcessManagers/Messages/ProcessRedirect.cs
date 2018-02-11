using System;
using GridDomain.Common;
using GridDomain.Node.Actors.CommandPipe.Messages;

namespace GridDomain.Node.Actors.ProcessManagers.Messages
{
    public class ProcessRedirect :  IProcessCompleted
    {
        public IMessageMetadataEnvelop MessageToRedirect { get; }
        public string ProcessId { get; }

        public ProcessRedirect(string processId, IMessageMetadataEnvelop messageToRedirect)
        {
            ProcessId = processId;
            MessageToRedirect = messageToRedirect;
        }
    }
}