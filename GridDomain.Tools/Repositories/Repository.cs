using System;
using System.Linq;
using CommonDomain;
using CommonDomain.Core;
using GridDomain.EventSourcing.Sagas.FutureEvents;
using GridDomain.Node.AkkaMessaging;

namespace GridDomain.Tools.Repositories
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
            var persistId = AggregateActorName.New<T>(aggr.Id).ToString();
            _eventRepository.Save(persistId, aggr.GetUncommittedEvents().Cast<object>().ToArray());
            aggr.ClearUncommittedEvents();
        }

        public T Load<T>(Guid id) where T : AggregateBase
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
    }
}