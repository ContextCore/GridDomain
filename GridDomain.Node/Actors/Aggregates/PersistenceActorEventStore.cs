using System.Collections.Generic;
using System.Threading.Tasks;
using Akka.Actor;
using GridDomain.EventSourcing;
using GridDomain.EventSourcing.CommonDomain;

namespace GridDomain.Node.Actors.Aggregates {
    class PersistenceActorEventStore<T> : IEventStore
    {
        private readonly IActorRef _actorRef;
        private readonly AggregateCommandExecutionContext _executionContext;

        public PersistenceActorEventStore(IActorRef actor, AggregateCommandExecutionContext ctx)
        {
            _executionContext = ctx;
            _actorRef = actor;
        }

        public Task Persist(IReadOnlyCollection<DomainEvent> events)
        {
            return _actorRef.Ask<T>(events);
        }

        public async Task Persist(IAggregate aggregate)
        {
            _executionContext.ProducedState = aggregate;
            if(aggregate.HasUncommitedEvents)
                await Persist(aggregate.GetUncommittedEvents());
        }
    }
}