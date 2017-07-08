using System;
using GridDomain.Common;
using GridDomain.EventSourcing;

namespace GridDomain.Scheduling.FutureEvents {
    public interface IFutureDomainEvent : ISourcedEvent, IHaveId, IHaveSagaId
    {
        string SourceName { get; }
        DateTime RaiseTime { get; }
    }
}