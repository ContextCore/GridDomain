using System;
using System.Linq;
using CommonDomain;
using CommonDomain.Core;
using GridDomain.EventSourcing;
using GridDomain.EventSourcing.Adapters;
using GridDomain.EventSourcing.Sagas.FutureEvents;
using GridDomain.Node.AkkaMessaging;
using GridDomain.Tools.Repositories.EventRepositories;
using GridDomain.Tools.Repositories.RawDataRepositories;

namespace GridDomain.Tools.Repositories.AggregateRepositories
{
    public class AggregateRepository : IDisposable
    {
        private readonly IRepository<DomainEvent> _eventRepository;
        private readonly EventsAdaptersCatalog _eventsAdaptersCatalog;

        public AggregateRepository(IRepository<DomainEvent> eventRepository, EventsAdaptersCatalog eventsAdaptersCatalog = null)
        {
            _eventsAdaptersCatalog = eventsAdaptersCatalog ?? new EventsAdaptersCatalog();
            _eventRepository = eventRepository;
        }

        public void Save<T>(T aggr) where T : IAggregate
        {
            var persistId = AggregateActorName.New<T>(aggr.Id).ToString();
            _eventRepository.Save(persistId, aggr.GetUncommittedEvents().Cast<DomainEvent>().ToArray());
            aggr.ClearUncommittedEvents();
        }

        public T LoadAggregate<T>(Guid id, int? version = null) where T : AggregateBase
        {
            var agr = Aggregate.Empty<T>(id);
            var persistId = AggregateActorName.New<T>(id).ToString();
            var events = _eventRepository.Load(persistId);

            var sequenceNr = version ?? events.Length;

            foreach (var e in events.Take(sequenceNr).SelectMany(e => _eventsAdaptersCatalog.Update(e)))
                ((IAggregate)agr).ApplyEvent(e);
            return agr;
        }

        public void Dispose()
        {
        }

        public static AggregateRepository New(string akkaWriteDbConnectionString, EventsAdaptersCatalog upgradeCatalog = null)
        {
            var rawSqlAkkaPersistenceRepository = new RawJournalRepository(akkaWriteDbConnectionString);
            var domainEventsRepository = new DomainEventsRepository(rawSqlAkkaPersistenceRepository);
            return new AggregateRepository(domainEventsRepository, upgradeCatalog);
        }
    }
}