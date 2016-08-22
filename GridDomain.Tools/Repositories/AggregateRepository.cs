using System;
using System.Linq;
using CommonDomain;
using CommonDomain.Core;
using GridDomain.EventSourcing;
using GridDomain.EventSourcing.Sagas.FutureEvents;
using GridDomain.Node.AkkaMessaging;
using GridDomain.Tools.Persistence;

namespace GridDomain.Tools.Repositories
{
    public class AggregateRepository : IDisposable
    {
        private readonly IRepository<DomainEvent> _eventRepository;

        public AggregateRepository(IRepository<DomainEvent> eventRepository)
        {
            _eventRepository = eventRepository;
        }

        public void Save<T>(T aggr) where T : IAggregate
        {
            var persistId = AggregateActorName.New<T>(aggr.Id).ToString();
            _eventRepository.Save(persistId, aggr.GetUncommittedEvents().Cast<DomainEvent>().ToArray());
            aggr.ClearUncommittedEvents();
        }

        public T LoadAggregate<T>(Guid id) where T : AggregateBase
        {
            var agr = Aggregate.Empty<T>(id);
            var persistId = AggregateActorName.New<T>(id).ToString();
            var events = _eventRepository.Load(persistId);
            foreach(var e in events)
                ((IAggregate)agr).ApplyEvent(e);
            return agr;
        }

        public void Dispose()
        {
            _eventRepository.Dispose();
        }


        public static AggregateRepository New(string akkaWriteDbConnectionString)
        {
            var rawSqlAkkaPersistenceRepository = new RawSqlAkkaPersistenceRepository(akkaWriteDbConnectionString);
            var domainEventsRepository = new DomainEventsRepository(rawSqlAkkaPersistenceRepository);
            return new AggregateRepository(domainEventsRepository);
        }
    }
}