using System;
using GridDomain.EventSourcing;

namespace GridDomain.Tests.Unit.AggregateLifetime.GracefulShutdown {
    public class WorkDone : DomainEvent
    {
        public string Value { get; }

        public WorkDone(string sourceId, string value):base(sourceId)
        {
            Value = value;
        }    
    }
}