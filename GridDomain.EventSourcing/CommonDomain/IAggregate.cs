using System;
using System.Collections;
using System.Collections.Generic;

namespace GridDomain.EventSourcing.CommonDomain
{
    public interface IAggregate
    {
        string Id { get; }
        void ApplyEvent(DomainEvent @event);
        void Commit(DomainEvent e);
        void CommitAll();
        bool HasUncommitedEvents { get; }
        
        IReadOnlyCollection<DomainEvent> GetUncommittedEvents();
        void InitEventStore(IEventStore store);
    }
}