using System;
using GridDomain.EventSourcing;

namespace GridDomain.Scheduling
{
    public class FutureEventCanceledEvent : DomainEvent
    {
        public FutureEventCanceledEvent(string futureEventId,
                                        string sourceId,
                                        string sourceName,
                                        DateTime? createdTime = null,
                                        string processId = null) : base(sourceId,  processId, createdTime: createdTime)
        {
            FutureEventId = futureEventId;
            SourceName = sourceName;
        }

        public string FutureEventId { get; }
        public string SourceName { get; }
    }
}