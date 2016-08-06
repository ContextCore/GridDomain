using System;
using CommonDomain;
using GridDomain.EventSourcing;

namespace GridDomain.Tools
{
    public interface IEventRepository : IDisposable
    {
        void Save<TAggregate>(Guid id, params DomainEvent[] messages) where TAggregate : IAggregate;
        DomainEvent[] Load<TAggregate>(Guid id) where TAggregate : IAggregate;
    }
}