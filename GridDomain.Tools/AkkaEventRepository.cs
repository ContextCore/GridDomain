using System;
using System.Linq;
using System.Threading;
using Akka.Actor;
using Akka.TestKit.NUnit;
using CommonDomain;
using CommonDomain.Core;
using GridDomain.EventSourcing;
using GridDomain.Node.AkkaMessaging;
using GridDomain.Node.Configuration.Akka;
using GridDomain.Tests.Framework.Persistence;

namespace GridDomain.Tools
{
    //Using testKit to easily locate all actor system exeptions 
    public class AkkaEventRepository : TestKit, IEventRepository
    {
        private static TimeSpan Timeout = TimeSpan.FromSeconds(5);

        public AkkaEventRepository(AkkaConfiguration config) : base(config.ToStandAloneSystemConfig())
        {
            
        }

        public void Save<TAggregate>(Guid id, params DomainEvent[] messages) where TAggregate : IAggregate
        {
            var persistActor = CreateEventsPersistActor<TAggregate>(id);

            foreach (var o in messages)
                persistActor.Ask<PersistEventsSaveActor.MessagePersisted>(o, Timeout);

            persistActor.Tell(PoisonPill.Instance);
        }

        private IActorRef CreateEventsPersistActor<TAggregate>(Guid id) where TAggregate : IAggregate
        {
            string persistId = AggregateActorName.New<TAggregate>(id).ToString();
            var props = Props.Create(() => new EventsPeristenceActor<TAggregate>(TestActor, persistId));
            var persistActor = Sys.ActorOf(props, Guid.NewGuid().ToString());
            return persistActor;
        }

        public DomainEvent[] Load<TAggregate>(Guid id) where TAggregate : IAggregate
        {
            var persistActor = CreateEventsPersistActor<TAggregate>(id);
            //load actor will notify caller automatically when it will load all events
            var msg = ExpectMsg<EventsPeristenceActor<TAggregate>.LoadComplete>(Timeout);
            persistActor.Tell(PoisonPill.Instance);
            return msg.Events.Cast<DomainEvent>().ToArray();
       } 
    }
}