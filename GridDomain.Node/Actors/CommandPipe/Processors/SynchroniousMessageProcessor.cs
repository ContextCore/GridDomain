using System.Threading.Tasks;
using Akka.Actor;

namespace GridDomain.Node.Actors.CommandPipe.Processors
{
    public class SynchronousMessageProcessor<T>: MessageProcessor<T>
    {
        public SynchronousMessageProcessor(IActorRef processor):base(processor)
        {
        }

        protected override async Task AttachToWorkInProgress(Task workInProgress, Task<T> currentWork)
        {
            await workInProgress;
            await currentWork;
        }

    }
}