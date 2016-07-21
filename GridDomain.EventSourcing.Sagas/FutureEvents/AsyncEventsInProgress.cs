using System;
using System.Threading.Tasks;

namespace GridDomain.EventSourcing.Sagas.FutureEvents
{
    public class AsyncEventsInProgress
    {
        public Guid InvocationId;

        public AsyncEventsInProgress(Task<DomainEvent[]> resultProducer, Guid invocationId)
        {
            ResultProducer = resultProducer;
            InvocationId = invocationId;
        }

        public Task<DomainEvent[]> ResultProducer { get; }
    }
}
