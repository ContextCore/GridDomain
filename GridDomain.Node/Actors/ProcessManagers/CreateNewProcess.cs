using System;
using GridDomain.Common;

namespace GridDomain.Node.Actors.ProcessManagers {
    public class CreateNewProcess
    {
        public IMessageMetadataEnvelop Message { get; }
        public Guid? EnforcedId { get; }

        public CreateNewProcess(IMessageMetadataEnvelop message, Guid? enforcedId = null)
        {
            Message = message;
            EnforcedId = enforcedId;
        }
    }
}