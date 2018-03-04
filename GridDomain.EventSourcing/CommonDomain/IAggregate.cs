using System;
using System.Collections;
using System.Collections.Generic;

namespace GridDomain.EventSourcing.CommonDomain
{
    public interface IAggregate
    {
        string Id { get; }
        void Apply(DomainEvent @event);
        bool HasUncommitedEvents { get; }
        IReadOnlyCollection<DomainEvent> GetUncommittedEvents();
        void ClearUncommitedEvents();
    }
}