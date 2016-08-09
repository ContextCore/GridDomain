using System;
using System.Linq;
using Akka.Actor;
using Akka.Configuration;
using Akka.TestKit.NUnit;
using GridDomain.Node;
using GridDomain.Node.Configuration.Akka;

namespace GridDomain.Tools.Repositories
{
    //Using testKit to easily locate all actor system exeptions 
    public class AkkaEventRepository : IEventRepository
    {
        private static readonly TimeSpan Timeout = TimeSpan.FromSeconds(5);
        private readonly ActorSystem _system;

        public AkkaEventRepository(ActorSystem config)
        {
            _system = config;
        }

        public static AkkaEventRepository New(AkkaConfiguration conf)
        {
            return new AkkaEventRepository(conf.CreateSystem());
        }


        public void Save(string id, params object[] messages)
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

        public object[] Load(string id)
        {
            var persistActor = CreateEventsPersistActor(id);
            //load actor will notify caller automatically when it will load all events
            var msg = persistActor.Ask<EventsRepositoryActor.Loaded>(new EventsRepositoryActor.Load(),Timeout).Result;
            persistActor.Tell(PoisonPill.Instance);
            return msg.Events.Cast<object>().ToArray();
       }

        public void Dispose()
        {

        }
    }
}