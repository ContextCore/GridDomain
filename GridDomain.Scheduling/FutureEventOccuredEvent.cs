using System;
using GridDomain.EventSourcing;

namespace GridDomain.Scheduling
{
    public class FutureEventOccuredEvent : DomainEvent
    {
        public FutureEventOccuredEvent(string id, string futureEventId, string sourceId) : base(sourceId,null,id)
        {
            FutureEventId = futureEventId;
        }

        public string FutureEventId { get; }
    }
}