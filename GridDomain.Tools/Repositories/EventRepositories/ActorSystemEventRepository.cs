using System;
using System.Linq;
using System.Threading.Tasks;
using Akka.Actor;
using GridDomain.EventSourcing;
using GridDomain.EventSourcing.Adapters;
using GridDomain.Node;
using GridDomain.Node.Configuration.Akka;

namespace GridDomain.Tools.Repositories.EventRepositories
{
    public class ActorSystemEventRepository : ActorSystemJournalRepository,
                                              IRepository<DomainEvent>
    {
        public ActorSystemEventRepository(ActorSystem config) : base(config) {}
        public Task Save(string id, params DomainEvent[] messages)
        {
            return base.Save(id, messages);
        }

        public async Task<DomainEvent[]> Load(string id)
        {
            var objects = await base.Load(id);
            return objects.Cast<DomainEvent>()
                          .ToArray();
        }
    }

    public class ActorSystemJournalRepository : IRepository<object>
    {
        private static readonly TimeSpan Timeout = TimeSpan.FromSeconds(1000);
        private readonly ActorSystem _system;

        public ActorSystemJournalRepository(ActorSystem config, bool requireJsonSerializer = true)
        {
            var ext = DomainEventsJsonSerializationExtensionProvider.Provider.Get(config);
            if (ext == null && requireJsonSerializer)
                throw new ArgumentNullException(nameof(ext),
                    $"Cannot get {typeof(DomainEventsJsonSerializationExtension).Name} extension");

            _system = config;
        }

        public static ActorSystemEventRepository New(AkkaConfiguration conf, EventsAdaptersCatalog eventsAdaptersCatalog)
        {
            var actorSystem = conf.CreateSystem();
            actorSystem.InitDomainEventsSerialization(eventsAdaptersCatalog);
            return new ActorSystemEventRepository(actorSystem);
        }

        public async Task Save(string id, params object[] messages)
        {
            var persistActor = CreateEventsPersistActor(id);

            foreach (var o in messages)
                await persistActor.Ask<EventsRepositoryActor.Persisted>(new EventsRepositoryActor.Persist(o), Timeout);

            var inbox = Inbox.Create(_system);

            inbox.Watch(persistActor);
            persistActor.Tell(PoisonPill.Instance);
            var terminated = await inbox.ReceiveAsync();

            if (!(terminated is Terminated))
                throw new InvalidOperationException();

        }

        private IActorRef CreateEventsPersistActor(string id)
        {
            var props = Props.Create(() => new EventsRepositoryActor(id));
            var persistActor = _system.ActorOf(props, Guid.NewGuid().ToString());
            return persistActor;
        }

        public async Task<object[]> Load(string id)
        {
            var persistActor = CreateEventsPersistActor(id);
            //load actor will notify caller automatically when it will load all events
            var msg =  await persistActor.Ask<EventsRepositoryActor.Loaded>(new EventsRepositoryActor.Load(),Timeout);
            persistActor.Tell(PoisonPill.Instance);
            return msg.Events.Cast<DomainEvent>().ToArray();
       }

        public void Dispose()
        {
        }
    }
}