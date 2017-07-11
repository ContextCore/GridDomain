using System.Threading.Tasks;
using Akka.Actor;

namespace GridDomain.Node.Actors.CommandPipe.Processors {
    public class ParrallelMessageProcessor<T> : MessageProcessor<T>
    {
        public ParrallelMessageProcessor(IActorRef processor) : base(processor)
        {
        }

        protected override Task AttachToWorkInProgress(Task workInProgress, Task<T> currentWork)
        {
            return Task.WhenAll(workInProgress, currentWork);
        }
    }
}