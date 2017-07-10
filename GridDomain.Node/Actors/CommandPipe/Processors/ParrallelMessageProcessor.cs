using System.Threading.Tasks;
using Akka.Actor;

namespace GridDomain.Node.Actors.CommandPipe.Processors {
    public class ParrallelMessageProcessor<T> : MessageActorProcessor
    {
        public ParrallelMessageProcessor(IActorRef processor) : base(processor)
        {
        }

        public override Task Process(object message, Task workInProgress = null)
        {
            return Task.WhenAll(workInProgress, AskActor<T>(message));
        }
    }
}