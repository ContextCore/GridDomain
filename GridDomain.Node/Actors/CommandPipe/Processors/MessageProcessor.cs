using System.Threading.Tasks;
using Akka.Actor;

namespace GridDomain.Node.Actors.CommandPipe.Processors {
    public abstract class MessageProcessor<T> : IMessageProcessor<T>, IMessageProcessor
    {
        protected MessageProcessor(IActorRef processor)
        {
            ActorRef = processor;
        }

        public Task<T> Process(object message, ref Task workInProgress)
        {
            var inProgress = ActorRef.Ask<T>(message);

            workInProgress = workInProgress == null ? inProgress : AttachToWorkInProgress(workInProgress, inProgress);

            return inProgress;
        }

        protected abstract Task AttachToWorkInProgress(Task workInProgress, Task<T> currentWork);

        Task IMessageProcessor.Process(object message, ref Task workInProgress)
        {
            return Process(message, ref workInProgress);
        }

        public IActorRef ActorRef { get; }
    }
}