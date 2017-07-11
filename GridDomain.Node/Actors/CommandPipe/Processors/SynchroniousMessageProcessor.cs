using System.Threading.Tasks;
using Akka.Actor;

namespace GridDomain.Node.Actors.CommandPipe.Processors
{
    public class SynchroniousMessageProcessor<T>: MessageProcessor<T>
    {
        public SynchroniousMessageProcessor(IActorRef processor):base(processor)
        {
        }

        protected override async Task AttachToWorkInProgress(Task workInProgress, Task<T> currentWork)
        {
            await workInProgress;
            await currentWork;
        }

    }
}