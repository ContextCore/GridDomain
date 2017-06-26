using System.Linq;
using Akka.Actor;
using Akka.TestKit.Xunit2;
using GridDomain.Tests.Common.Configuration;
using GridDomain.Tools.Repositories;
using Xunit;

namespace GridDomain.Tests.Unit.Tools.Repositories
{
    public class EventPersistenceActor_Tests : TestKit
    {
        public EventPersistenceActor_Tests() : base(new AutoTestAkkaConfiguration().ToStandAloneInMemorySystemConfig()) {}

        private IActorRef CreateActor(string persistenceId)
        {
            return Sys.ActorOf(Props.Create(() => new EventsRepositoryActor(persistenceId)));
        }

        private EventsRepositoryActor.Loaded LoadEvents(IActorRef actor)
        {
            var res = actor.Ask<EventsRepositoryActor.Loaded>(new EventsRepositoryActor.Load()).Result;
            return res;
        }

        private EventsRepositoryActor.Persisted Save(IActorRef actor, object payload)
        {
            var persisted = actor.Ask<EventsRepositoryActor.Persisted>(new EventsRepositoryActor.Persist(payload)).Result;
            return persisted;
        }

        [Fact]
        public void When_acor_receives_persist_request_it_persist_payload()
        {
            var actor = CreateActor("2");
            var payload = "123";
            Save(actor, payload);
            actor.Tell(PoisonPill.Instance);

            actor = CreateActor("2");
            var loaded = LoadEvents(actor);

            Assert.Equal(payload, loaded.Events.FirstOrDefault());
        }

        [Fact]
        public void When_acor_receives_persist_request_it_responses_with_initial_persisted_payload()
        {
            var actor = CreateActor("2");
            var payload = "123";
            var persisted = Save(actor, payload);
            Assert.Equal(payload, persisted.Payload);
        }

        [Fact]
        public void When_actor_is_created_and_asked_for_load_response_contains_persisteneId()
        {
            var actor = CreateActor("1");
            var res = LoadEvents(actor);
            Assert.Equal("1", res.PersistenceId);
        }

        [Fact]
        public void When_actor_is_created_and_asked_for_load_response_is_empty()
        {
            var actor = CreateActor("1");
            var res = LoadEvents(actor);
            Assert.Empty(res.Events);
        }

        [Fact]
        public void When_actor_is_created_it_does_not_send_anything()
        {
            CreateActor("1");
            ExpectNoMsg(500);
        }
    }
}