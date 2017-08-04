using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Akka.Actor;
using GridDomain.EventSourcing;
using GridDomain.EventSourcing.Adapters;
using GridDomain.Node;
using GridDomain.Node.Configuration.Akka;
using GridDomain.Node.Serializers;

namespace GridDomain.Tools.Repositories.EventRepositories
{
    public class ActorSystemJournalRepository : IRepository<object>
    {
        private static readonly TimeSpan Timeout = Debugger.IsAttached ? TimeSpan.FromHours(1) : TimeSpan.FromSeconds(10);
        private readonly ActorSystem _system;

        public ActorSystemJournalRepository(ActorSystem config, bool requireJsonSerializer = true)
        {
            var ext = DomainEventsJsonSerializationExtensionProvider.Provider.Get(config);
            if (ext == null && requireJsonSerializer)
                throw new ArgumentNullException(nameof(ext),
                                                $"Cannot get {typeof(DomainEventsJsonSerializationExtension).Name} extension");

            _system = config;
        }

        public async Task Save(string id, params object[] messages)
        {
            var persistActor = CreateEventsPersistActor(id);
            try
            {
                foreach (var o in messages)
                    await persistActor.Ask<EventsRepositoryActor.Persisted>(new EventsRepositoryActor.Persist(o), Timeout);
            }
            finally
            {
                await TerminateActor(persistActor);
            }
        }

        public async Task<object[]> Load(string id)
        {
            var persistActor = CreateEventsPersistActor(id);
            try
            {
                //load actor will notify caller automatically when it will load all events
                var msg = await persistActor.Ask<EventsRepositoryActor.Loaded>(new EventsRepositoryActor.Load(), Timeout);
                return msg.Events.Cast<DomainEvent>().ToArray();
            }
            finally
            {
                await TerminateActor(persistActor);
            }
        }

        public void Dispose() {}

        public static ActorSystemEventRepository New(AkkaConfiguration conf, EventsAdaptersCatalog eventsAdaptersCatalog)
        {
            var actorSystem = conf.CreateSystem();
            actorSystem.InitDomainEventsSerialization(eventsAdaptersCatalog);
            return new ActorSystemEventRepository(actorSystem);
        }

        private async Task TerminateActor(IActorRef persistActor)
        {
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
            var persistActor = _system.ActorOf(props, "Test_events_persist_actor_" + Guid.NewGuid());
            return persistActor;
        }
    }
}