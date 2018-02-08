using System;
using System.Threading.Tasks;
using Akka.Actor;
using GridDomain.Common;
using GridDomain.Node.Actors.EventSourced.Messages;
using GridDomain.Node.Actors.PersistentHub;
using Xunit;

namespace GridDomain.Tests.Unit.AggregateLifetime
{
    public abstract class PersistentHubChildrenLifetimeTest : NodeTestKit
    {
        public PersistentHubChildrenLifetimeTest(PersistentHubFixture fixture): base(fixture)
        {
            Infrastructure = fixture.Infrastructure;
            var actorOfAsTestActorRef = ActorOfAsTestActorRef<PersistentHubActor>(Infrastructure.CreateHubProps(Node.System),
                                                                                  "TestHub_" + Guid.NewGuid());
            _hub = actorOfAsTestActorRef.UnderlyingActor;
            _hubRef = actorOfAsTestActorRef.Ref;
        }

        private IPersistentActorTestsInfrastructure Infrastructure { get; }

        private async Task When_hub_creates_a_child()
        {
            _hubRef.Tell(Infrastructure.ChildCreateMessage);
            await Task.Delay(200);
        }

        private async Task And_command_for_child_is_sent()
        {
            _hubRef.Tell(Infrastructure.ChildActivateMessage);
            await Task.Delay(200);
        }

        private readonly PersistentHubActor _hub;
        private readonly IActorRef _hubRef;

        private IActorRef Child => 
            Sys.ActorSelection(_hubRef,_hub.GetChildActorName(Infrastructure.ChildId)).ResolveOne(TimeSpan.FromSeconds(1)).Result;

        private async Task<HealthStatus> PingChild(string payload)
        {
            return await Child.Ask(new CheckHealth(payload), TimeSpan.FromSeconds(1)) as HealthStatus;
        }

        [Fact]
        public async Task PersistenceHubOnChildCreation()
        {
            //When_Hub_creates_a_child()
            await When_hub_creates_a_child();
            //Time_to_life_is_set_and_can_differs_by_check_time()
            Assert.NotNull(Child);
        }

        [Fact]
        public async Task When_child_became_inactive_too_long()
        {
            await When_hub_creates_a_child();
            var childRef = Child;
            Watch(childRef);
            //It_should_be_terminated()
            FishForMessage<Terminated>(m => m.ActorRef.Path == childRef.Path,
                                      TimeSpan.FromSeconds(10));
        }

        [Fact]
        public async Task When_child_has_activity_while_living_Then_it_should_exist()
        {
            await When_hub_creates_a_child();

            //TODO: replace sleep with dateTime manipulations via DateTimeFacade
            await Task.Delay(200);
            await And_command_for_child_is_sent();
            Assert.NotNull(Child);
            // Child_should_exist()
            var payload = "child ping";
            Assert.Equal(payload, (await PingChild(payload)).Payload);
        }

        [Fact]
        public async Task When_child_revives()
        {
            await When_hub_creates_a_child();
            Watch(Child);
            var actorToWatchPath = Child.Path;

            //And_it_is_not_active_until_lifetime_period_is_expired
            FishForMessage<Terminated>(t => t.ActorRef.Path == actorToWatchPath,TimeSpan.FromSeconds(5));
            await And_command_for_child_is_sent();
            
            //Hub_should_contains_child()
            Assert.NotNull(Child);
            
            //It_should_be_restored_after_command_execution()
            var payload = "child ping";
            Assert.Equal(payload, (await PingChild(payload)).Payload);
        }
    }
}