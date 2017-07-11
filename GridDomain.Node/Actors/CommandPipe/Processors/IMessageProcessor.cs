using System.Threading.Tasks;
using Akka.Actor;

namespace GridDomain.Node.Actors.CommandPipe.Processors {
    public interface IMessageProcessor<T>
    {
        Task<T> Process(object message, ref Task workInProgress);
        IActorRef ActorRef { get; }
    }

    public interface IMessageProcessor
    {
        Task Process(object message, ref Task workInProgress);
        IActorRef ActorRef { get; }
    }
}