using System.Collections.Generic;
using Akka.Actor;
using GridDomain.Common;
using GridDomain.CQRS;

namespace GridDomain.Node.Actors.ProcessManagers {
    class ProcessTransitionContext<TState>
    {
        public IMessageMetadataEnvelop processingEnvelop;
        public TState pendingState;
        public IReadOnlyCollection<ICommand> producedCommands = new ICommand[] { };
        public IActorRef processingMessageSender;

        public void Clear()
        {
            processingEnvelop = null;
            pendingState = default(TState);
            producedCommands = new ICommand[] { };
            processingMessageSender = null;
        }
    }
}