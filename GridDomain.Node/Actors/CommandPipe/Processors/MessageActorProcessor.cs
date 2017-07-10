using System.Threading.Tasks;
using Akka.Actor;

namespace GridDomain.Node.Actors.CommandPipe.Processors
{
    public abstract class MessageActorProcessor : IMessageProcessor
    {
        protected MessageActorProcessor(IActorRef actorRef)
        {
            _actorRef = actorRef;
        }

        private readonly IActorRef _actorRef;
        public IActorRef ActorRef => _actorRef;

        public abstract Task Process(object message, Task workInProgress = null);
        protected Task<T> AskActor<T>(object message)
        {
            return _actorRef.Ask<T>(message);
        }
    }
}