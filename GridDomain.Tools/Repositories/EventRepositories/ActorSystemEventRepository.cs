using System.Linq;
using System.Threading.Tasks;
using Akka.Actor;
using GridDomain.EventSourcing;

namespace GridDomain.Tools.Repositories.EventRepositories
{
    public class ActorSystemEventRepository : ActorSystemJournalRepository,
                                              IRepository<DomainEvent>
    {
        public ActorSystemEventRepository(ActorSystem system) : base(system) {}

        public Task Save(string id, params DomainEvent[] messages)
        {
            return base.Save(id, messages);
        }

        public new async Task<DomainEvent[]> Load(string id)
        {
            var objects = await base.Load(id);
            return objects.Cast<DomainEvent>().ToArray();
        }
    }
}