using System;
using System.Linq;
using System.Threading.Tasks;


using GridDomain.EventSourcing;
using GridDomain.EventSourcing.Adapters;
using GridDomain.EventSourcing.CommonDomain;
using GridDomain.Node.AkkaMessaging;
using GridDomain.Tools.Repositories.EventRepositories;
using GridDomain.Tools.Repositories.RawDataRepositories;

namespace GridDomain.Tools.Repositories.AggregateRepositories
{
    public class AggregateRepository : IDisposable
    {
        private readonly IRepository<DomainEvent> _eventRepository;
        private readonly EventsAdaptersCatalog _eventsAdaptersCatalog;

        public AggregateRepository(IRepository<DomainEvent> eventRepository,
                                   EventsAdaptersCatalog eventsAdaptersCatalog = null)
        {
            _eventsAdaptersCatalog = eventsAdaptersCatalog ?? new EventsAdaptersCatalog();
            _eventRepository = eventRepository;
        }

        public void Dispose() {}

        public async Task Save<T>(T aggr) where T : Aggregate
        {
            var persistId = AggregateActorName.New<T>(aggr.Id).ToString();
            await _eventRepository.Save(persistId, aggr.GetDomainEvents());
            aggr.PersistAll();
        }

        public async Task<T> LoadAggregate<T>(Guid id) where T : AggregateBase
        {
            var agr = Aggregate.Empty<T>(id);
            var persistId = AggregateActorName.New<T>(id).ToString();
            var events = await _eventRepository.Load(persistId);
            foreach (var e in events.SelectMany(e => _eventsAdaptersCatalog.Update(e)))
                ((IAggregate) agr).ApplyEvent(e);
            return agr;
        }

        public static AggregateRepository New(string akkaWriteDbConnectionString, EventsAdaptersCatalog upgradeCatalog = null)
        {
            var rawSqlAkkaPersistenceRepository = new RawJournalRepository(akkaWriteDbConnectionString);
            var domainEventsRepository = new DomainEventsRepository(rawSqlAkkaPersistenceRepository);
            return new AggregateRepository(domainEventsRepository, upgradeCatalog);
        }
    }
}