using System;
using System.Linq;
using CommonDomain;
using CommonDomain.Core;
using GridDomain.EventSourcing;
using GridDomain.EventSourcing.Sagas.FutureEvents;

namespace GridDomain.Tools
{
    public class Repository : IRepository
    {
        private readonly IEventRepository _eventRepository;

        public Repository(IEventRepository eventRepository)
        {
            _eventRepository = eventRepository;
        }

        public void Save<T>(T aggr) where T : IAggregate
        {
            _eventRepository.Save<T>(aggr.Id, aggr.GetUncommittedEvents().Cast<DomainEvent>().ToArray());
            aggr.ClearUncommittedEvents();
        }

        public T Load<T>(Guid id) where T : AggregateBase
        {
            var agr = Aggregate.Empty<T>(id);
            var events = _eventRepository.Load<T>(id);
            foreach(var e in events)
                ((IAggregate)agr).ApplyEvent(e);
            return agr;
        }

        public void Dispose()
        {
            _eventRepository.Dispose();
        }
    }
}