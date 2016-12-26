using System.Linq;
using Akka.Actor;
using Akka.TestKit.NUnit3;
using GridDomain.Tests.Framework.Configuration;
using GridDomain.Tools.Repositories;
using NUnit.Framework;

namespace GridDomain.Tests.Unit.Tools.Repositories
{
    [TestFixture]
    public class EventPersistenceActor_Tests : TestKit
    {

        public EventPersistenceActor_Tests():base(new AutoTestAkkaConfiguration().ToStandAloneInMemorySystemConfig())
        {
            
        }
        [Test]
        public void When_actor_is_created_it_does_not_send_anything()
        {
            var actor = CreateActor("1");
            ExpectNoMsg(500);
        }

        [Test]
        public void When_actor_is_created_and_asked_for_load_response_is_empty()
        {
            var actor = CreateActor("1");
            var res = LoadEvents("1");
            CollectionAssert.IsEmpty(res.Events);
        }

        [Test]
        public void When_actor_is_created_and_asked_for_load_response_contains_persisteneId()
        {
            var actor = CreateActor("1");
            var res = LoadEvents("1");
            Assert.AreEqual("1",res.PersistenceId);
        }

        [Test]
        public void When_acor_receives_persist_request_it_responses_with_initial_persisted_payload()
        {
            var actor = CreateActor("2");
            var payload = "123";
            var persisted = Save(actor, payload);
            Assert.AreEqual(payload, persisted.Payload);
        }

        [Test]
        public void When_acor_receives_persist_request_it_persist_payload()
        {
            var actor = CreateActor("2");
            var payload = "123";
            Save(actor, payload);

            var loaded = LoadEvents("2");
            Assert.AreEqual(payload, loaded.Events.FirstOrDefault());
        }

        private IActorRef CreateActor(string persistenceId)
        {
            return Sys.ActorOf(Props.Create(() => new EventsRepositoryActor(persistenceId)));
        }

        private EventsRepositoryActor.Loaded LoadEvents(string persistenceId)
        {
            var actor = CreateActor(persistenceId);
            var res = actor.Ask<EventsRepositoryActor.Loaded>(new EventsRepositoryActor.Load()).Result;
            return res;
        }

        private EventsRepositoryActor.Persisted Save(IActorRef actor, object payload)
        {
            var persisted = actor.Ask<EventsRepositoryActor.Persisted>(new EventsRepositoryActor.Persist(payload)).Result;
            return persisted;
        }
    }
}