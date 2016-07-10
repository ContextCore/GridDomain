using System.Threading.Tasks;
using GridDomain.CQRS;
using GridDomain.EventSourcing;

namespace GridDomain.Node.FutureEvents
{
    public class AsyncMethodStarted
    {

        public AsyncMethodStarted(Task<DomainEvent[]> resultProducer, ICommand command = null)
        {
            ResultProducer = resultProducer;
            Command = command;
        }

        public Task<DomainEvent[]> ResultProducer { get;}
        public ICommand Command { get; }
    }
}