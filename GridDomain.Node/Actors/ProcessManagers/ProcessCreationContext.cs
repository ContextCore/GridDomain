using Akka.Actor;
using GridDomain.Common;

namespace GridDomain.Node.Actors.ProcessManagers {
    class ProcessCreationContext<TState>
    {
        public TState pendingState;
        public IMessageMetadataEnvelop processingMessage;
        public IActorRef processingMessageSender;

        public void Clear()
        {
            pendingState = default(TState);
            processingMessage = null;
            processingMessageSender = null;
        }
    }
}