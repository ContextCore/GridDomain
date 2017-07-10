using System.Threading.Tasks;
using Akka.Actor;

namespace GridDomain.Node.Actors.CommandPipe.Processors {
    public class SynchroniousMessageProcessor<T>: IMessageProcessor<T>
    {
        public SynchroniousMessageProcessor(IActorRef processor)
        {
            ActorRef = processor;
        }

        public Task<T> Process(object message, ref Task workInProgress)
        {
            var inProgress = ActorRef.Ask<T>(message);

            if(workInProgress == null || workInProgress.IsCompleted)
                workInProgress = inProgress;
            else
                workInProgress = workInProgress.ContinueWith(t => inProgress);

            return inProgress;
        }

        public IActorRef ActorRef { get; }
    }
}