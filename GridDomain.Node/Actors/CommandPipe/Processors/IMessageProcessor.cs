using System.Threading.Tasks;
using Akka.Actor;

namespace GridDomain.Node.Actors.CommandPipe.Processors {
    public interface IMessageProcessor
    {
        Task Process(object message, Task workInProgress = null);
        IActorRef ActorRef { get; }
    }
}