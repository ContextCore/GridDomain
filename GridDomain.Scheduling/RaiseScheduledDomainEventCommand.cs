using System;
using GridDomain.CQRS;

namespace GridDomain.Scheduling
{
    public class RaiseScheduledDomainEventCommand : Command
    {
        public RaiseScheduledDomainEventCommand(string futureEventId, string aggregateId, string id) : base(id, aggregateId)
        {
            FutureEventId = futureEventId;
        }

        public string FutureEventId { get; }
    }
}