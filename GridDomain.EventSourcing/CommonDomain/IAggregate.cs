using System;
using System.Collections;
using System.Collections.Generic;

namespace GridDomain.EventSourcing.CommonDomain
{
    public interface IAggregate
    {
        Guid Id { get; }
        int Version { get; }
        void ApplyEvent(DomainEvent @event);
        IReadOnlyCollection<DomainEvent> GetUncommittedEvents();
        void ClearUncommitedEvents();
        IMemento GetSnapshot();
    }
}