using System.Threading.Tasks;
using Akka.Actor;

namespace GridDomain.Node.Actors.CommandPipe.Processors {
    public class SynchroniousMessageProcessor<T>: MessageActorProcessor
    {
        public SynchroniousMessageProcessor(IActorRef processor):base(processor)
        {
        }

        public override Task Process(object message, Task workInProgress = null)
        {
            if(workInProgress == null || workInProgress.IsCompleted)
                return AskActor<T>(message);

            return workInProgress.ContinueWith(t => AskActor<T>(message));
        }
    }
}