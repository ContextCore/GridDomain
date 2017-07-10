using System.Threading.Tasks;
using Akka.Actor;

namespace GridDomain.Node.Actors.CommandPipe.Processors
{
    public abstract class MessageActorProcessor : IMessageProcessor
    {
        protected MessageActorProcessor(IActorRef actorRef)
        {
            ActorRef = actorRef;
        }
        public IActorRef ActorRef { get; }

        public abstract Task Process(object message, Task workInProgress = null);
        protected Task<T> AskActor<T>(object message)
        {
            return ActorRef.Ask<T>(message);
        }
    }
}