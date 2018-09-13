using System;
using System.Collections;
using System.Collections.Generic;
using GridDomain.CQRS;

namespace GridDomain.EventSourcing.CommonDomain
{
  
    public interface IAggregate : IEventSourced, IAggregateCommandHandler
    {
    }

    public interface IAggregateCommandHandler : ICommandHandler<ICommand, IReadOnlyCollection<DomainEvent>>
    {
        
    }
   
    public interface IEventSourced
    {
      string Id { get; }
      void Apply(DomainEvent @event); 
    }

    public interface IEventList
    {
        void Clear();
        IReadOnlyCollection<DomainEvent> Events { get; }
    }

    public interface IEventPersistentObserver
    {
        void OnPersist(DomainEvent ev);
    }
}