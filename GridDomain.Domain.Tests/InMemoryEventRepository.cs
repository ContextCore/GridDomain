using System;
using System.Collections.Generic;
using System.Linq;
using CommonDomain;
using CommonDomain.Persistence;
using GridDomain.EventSourcing;
using NEventStore;

namespace GridDomain.Domain.Tests
{
    public class InMemoryEventRepository : IRepository
    {
        private readonly IConstructAggregates _aggregateFactory;

        private readonly IDictionary<Guid, List<DomainEvent>> _aggregates = new Dictionary<Guid, List<DomainEvent>>();

        public InMemoryEventRepository(IConstructAggregates aggregateFactory)
        {
            this._aggregateFactory = aggregateFactory;
            ProducedEvents = new List<DomainEvent>();
        }

        public void AddEvent(DomainEvent e)
        {
            var events = GetOrAddEventList(e.SourceId);
            events.Add(e);
        }

        private List<DomainEvent> GetOrAddEventList(Guid guid)
        {
            List<DomainEvent> events;
            var sourceId = guid;
            if (!_aggregates.TryGetValue(sourceId, out events))
                _aggregates[sourceId] = events = new List<DomainEvent>();
            return events;
        }

        public List<DomainEvent> ProducedEvents { get; private set; }

        public TAggregate GetById<TAggregate>(Guid id) where TAggregate : class, IAggregate
        {
            return GetById<TAggregate>(Bucket.Default, id, 0);
        }

        public TAggregate GetById<TAggregate>(Guid id, int version) where TAggregate : class, IAggregate
        {
            return GetById<TAggregate>(Bucket.Default, id, 0);
        }

        public TAggregate GetById<TAggregate>(string bucketId, Guid id) where TAggregate : class, IAggregate
        {
            return GetById<TAggregate>(Bucket.Default, id, 0);
        }

        public TAggregate GetById<TAggregate>(string bucketId, Guid id, int version) where TAggregate : class, IAggregate
        {
            var aggregate = _aggregateFactory.Build(typeof(TAggregate), id, null);

            Hydrate(aggregate);

            return (TAggregate)aggregate;
        }

        private void Hydrate(IAggregate aggregate)
        {
            GetOrAddEventList(aggregate.Id).ForEach(aggregate.ApplyEvent);
            aggregate.ClearUncommittedEvents();
        }

        public void Save(IAggregate aggregate, Guid commitId, Action<IDictionary<string, object>> updateHeaders)
        {
            Save(Bucket.Default, aggregate, commitId, updateHeaders);
        }

        public void Save(string bucketId, IAggregate aggregate, Guid commitId, Action<IDictionary<string, object>> updateHeaders)
        {
            ProducedEvents.AddRange(aggregate.GetUncommittedEvents().Cast<DomainEvent>());
            GetOrAddEventList(aggregate.Id).AddRange(aggregate.GetUncommittedEvents().Cast<DomainEvent>());
        }

        public void Dispose()
        {
            // no op
        }
    }
}