using System;
using GridDomain.EventSourcing;

namespace GridDomain.Scheduling
{
    public class FutureEventOccuredEvent : DomainEvent
    {
        public FutureEventOccuredEvent(string sourceId, string futureEventId, string id=null) : base(sourceId,null,id)
        {
            FutureEventId = futureEventId;
        }

        public string FutureEventId { get; }
    }
}