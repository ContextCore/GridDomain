using System;
using System.Linq;
using System.Threading;
using Akka.Actor;
using Akka.Configuration;
using Akka.TestKit.NUnit;
using CommonDomain;
using CommonDomain.Core;
using GridDomain.EventSourcing;
using GridDomain.Node.AkkaMessaging;
using GridDomain.Node.Configuration.Akka;

namespace GridDomain.Tools
{
    //Using testKit to easily locate all actor system exeptions 
    public class AkkaEventRepository : TestKit, IEventRepository
    {
        private static TimeSpan Timeout = TimeSpan.FromSeconds(5);

        public AkkaEventRepository(Config config) : base(config)
        {
            
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
            var persistActor = Sys.ActorOf(props, Guid.NewGuid().ToString());
            return persistActor;
        }

        public object[] Load(string id)
        {
            var persistActor = CreateEventsPersistActor(id);
            //load actor will notify caller automatically when it will load all events
            var msg = ExpectMsg<EventsRepositoryActor.Loaded>(Timeout);
            persistActor.Tell(PoisonPill.Instance);
            return msg.Events.Cast<object>().ToArray();
       } 
    }
}