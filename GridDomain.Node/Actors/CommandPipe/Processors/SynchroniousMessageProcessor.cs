using System.Threading.Tasks;
using Akka.Actor;

namespace GridDomain.Node.Actors.CommandPipe.Processors
{

    public abstract class MessageProcessor<T> : IMessageProcessor<T>, IMessageProcessor
    {
        protected MessageProcessor(IActorRef processor)
        {
            ActorRef = processor;
        }

        public Task<T> Process(object message, ref Task workInProgress)
        {
            var inProgress = ActorRef.Ask<T>(message);

            if(workInProgress == null || workInProgress.IsCompleted)
                workInProgress = inProgress;
            else
                workInProgress = AttachToWorkInProgress(workInProgress, inProgress);

            return inProgress;
        }

        protected abstract Task AttachToWorkInProgress(Task workInProgress, Task<T> inProgress);

        Task IMessageProcessor.Process(object message, ref Task workInProgress)
        {
            return Process(message, ref workInProgress);
        }

        public IActorRef ActorRef { get; }
    }
    public class SynchroniousMessageProcessor<T>: MessageProcessor<T>
    {
        public SynchroniousMessageProcessor(IActorRef processor):base(processor)
        {
        }

        protected override Task AttachToWorkInProgress(Task workInProgress, Task<T> inProgress)
        {
            return workInProgress.ContinueWith(t => inProgress);
        }

    }
}