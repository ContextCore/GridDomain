using System;
using GridDomain.Processes;

namespace GridDomain.Node.Actors.ProcessManagers.Exceptions
{
    internal class ProcessAlreadyStartedException : Exception
    {
        public object StartMessage { get; }
        public IProcessState ExistingState { get; }

        public ProcessAlreadyStartedException(IProcessState existingState, object startMessage)
        {
            StartMessage = startMessage;
            ExistingState = existingState;
        }
    }
}