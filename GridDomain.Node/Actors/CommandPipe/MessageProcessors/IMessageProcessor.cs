using System.Threading.Tasks;
using Akka.Actor;

namespace GridDomain.Node.Actors.CommandPipe.MessageProcessors {
    public interface IMessageProcessor<T>
    {
        Task<T> Process(object message, ref Task workInProgress);
    }

    public interface IMessageProcessor
    {
        Task Process(object message, ref Task workInProgress);
    }

    public class MessageProcessor
    {
        IMessageProcessor FireAndForget(IActorRef actor)
        {
            return new FireAndForgetMessageProcessor(actor);
        }
        
    }
}