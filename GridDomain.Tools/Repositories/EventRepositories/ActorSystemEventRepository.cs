using System.Linq;
using System.Threading.Tasks;
using Akka.Actor;
using GridDomain.EventSourcing;
using GridDomain.Node.AkkaMessaging;

namespace GridDomain.Tools.Repositories.EventRepositories
{
    public class ActorSystemEventRepository : ActorSystemJournalRepository,
                                              IRepository<DomainEvent>
    {
        public ActorSystemEventRepository(ActorSystem system) : base(system) {}

        public Task Save(string aggregateId, params DomainEvent[] messages)
        {
            return base.Save(aggregateId, messages);
        }

        public Task Save<TAggregate>(string aggregateId, params DomainEvent[] events)
        {
            return Save(EntityActorName.New<TAggregate>(aggregateId).ToString(), events);
        }

        public new async Task<DomainEvent[]> Load(string persistenceId)
        {
            var objects = await base.Load(persistenceId);
            return objects.Cast<DomainEvent>().ToArray();
        }

        public  async Task<DomainEvent[]> Load<TAggregate>(string persistenceId)
        {
            return await Load(EntityActorName.New<TAggregate>(persistenceId).ToString());
        }
    }
}