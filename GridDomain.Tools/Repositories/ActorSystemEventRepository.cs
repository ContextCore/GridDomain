using System;
using System.Linq;
using Akka.Actor;
using Akka.Configuration;
using Akka.TestKit.NUnit;
using GridDomain.EventSourcing;
using GridDomain.Node;
using GridDomain.Node.Configuration.Akka;

namespace GridDomain.Tools.Repositories
{
    //Using testKit to easily locate all actor system exeptions 
    public class ActorSystemEventRepository : IRepository<DomainEvent>
    {
        private static readonly TimeSpan Timeout = TimeSpan.FromSeconds(10);
        private readonly ActorSystem _system;

        public ActorSystemEventRepository(ActorSystem config)
        {
            _system = config;
        }

        public static ActorSystemEventRepository New(AkkaConfiguration conf)
        {
            return new ActorSystemEventRepository(conf.CreateSystem());
        }


        public void Save(string id, params DomainEvent[] messages)
        {
            var persistActor = CreateEventsPersistActor(id);

            foreach (var o in messages)
                persistActor.Ask<EventsRepositoryActor.Persisted>(new EventsRepositoryActor.Persist(o), Timeout).Wait();

            persistActor.Tell(PoisonPill.Instance);
        }

        private IActorRef CreateEventsPersistActor(string id)
        {
            var props = Props.Create(() => new EventsRepositoryActor(id));
            var persistActor = _system.ActorOf(props, Guid.NewGuid().ToString());
            return persistActor;
        }

        public DomainEvent[] Load(string id)
        {
            var persistActor = CreateEventsPersistActor(id);
            //load actor will notify caller automatically when it will load all events
            var msg = persistActor.Ask<EventsRepositoryActor.Loaded>(new EventsRepositoryActor.Load(),Timeout).Result;
            persistActor.Tell(PoisonPill.Instance);
            return msg.Events.Cast<DomainEvent>().ToArray();
       }

        public void Dispose()
        {

        }
    }
}