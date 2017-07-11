using System.Threading.Tasks;

namespace GridDomain.Node.Actors.CommandPipe.Processors {
    public interface IMessageProcessor<T>
    {
        Task<T> Process(object message, ref Task workInProgress);
    }

    public interface IMessageProcessor
    {
        Task Process(object message, ref Task workInProgress);
    }
}