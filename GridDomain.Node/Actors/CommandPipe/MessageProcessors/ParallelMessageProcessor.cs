using System.Threading.Tasks;
using Akka.Actor;

namespace GridDomain.Node.Actors.CommandPipe.MessageProcessors {
    public class ParallelMessageProcessor<T> : MessageProcessor<T>
    {
        public ParallelMessageProcessor(IActorRef processor) : base(processor)
        {
        }

        protected override Task AttachToWorkInProgress(Task workInProgress, Task<T> currentWork)
        {
            return Task.WhenAll(workInProgress, currentWork);
        }
    }
}