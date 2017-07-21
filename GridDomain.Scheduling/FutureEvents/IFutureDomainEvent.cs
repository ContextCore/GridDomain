using System;
using GridDomain.Common;
using GridDomain.EventSourcing;

namespace GridDomain.Scheduling.FutureEvents {
    public interface IFutureDomainEvent : ISourcedEvent, IHaveId, IHaveProcessId
    {
        string SourceName { get; }
        DateTime RaiseTime { get; }
    }
}