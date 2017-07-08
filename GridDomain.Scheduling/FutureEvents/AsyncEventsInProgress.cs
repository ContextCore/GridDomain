using System;
using System.Threading.Tasks;
using GridDomain.EventSourcing;

namespace GridDomain.Scheduling.FutureEvents
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