using System;
using System.Threading;
using System.Threading.Tasks;
using Akka.Actor;
using GridDomain.Common;
using GridDomain.Node;
using GridDomain.Node.Actors;
using Serilog.Events;
using Xunit;
using Xunit.Abstractions;

namespace GridDomain.Tests.XUnit.Aggregate_Sagas_actor_lifetime
{
    public abstract class PersistentHubChildrenLifetimeTest: NodeTestKit
    {
        private IPersistentActorTestsInfrastructure Infrastructure { get; }

        public PersistentHubChildrenLifetimeTest(ITestOutputHelper output, PersistentHubFixture fixture)
            : base(output, fixture)
        {
            Infrastructure = fixture.Infrastructure;
            var actorOfAsTestActorRef = ActorOfAsTestActorRef<PersistentHubActor>(Infrastructure.CreateHubProps(Node.System), "TestHub_" + Guid.NewGuid());
            Hub = actorOfAsTestActorRef.UnderlyingActor;
            HubRef = actorOfAsTestActorRef.Ref;
        }

        private void When_hub_creates_a_child()
        {
            HubRef.Tell(Infrastructure.ChildCreateMessage);
            Thread.Sleep(200);
        }

        private void And_it_is_not_active_until_lifetime_period_is_expired()
        {
            Thread.Sleep(3000);
        }

        private void And_command_for_child_is_sent()
        {
            HubRef.Tell(Infrastructure.ChildActivateMessage);
            Thread.Sleep(200);
        }

        private readonly PersistentHubActor Hub;
        private readonly IActorRef HubRef;
        private ChildInfo Child => Hub.Children[Infrastructure.ChildId];

        private async Task<HealthStatus> PingChild(string payload)
        {
            return await Child.Ref.Ask(new CheckHealth(payload), TimeSpan.FromSeconds(1)) as HealthStatus;
        }

        [Fact]
        public async Task When_child_has_activity_while_living_Then_it_should_exist()
        {
            When_hub_creates_a_child();
            var initialTimeToLife = Child.ExpiresAt;

            //TODO: replace sleep with dateTime manipulations via DateTimeFacade
            Thread.Sleep(200);

            And_command_for_child_is_sent();
            //Hub_should_contains_child()
            Assert.True(Hub.Children.ContainsKey(Infrastructure.ChildId));
            //Child_lifeTime_should_be_prolongated()
            Assert.True(Child.ExpiresAt >= initialTimeToLife);
            // Child_should_exist()
            var payload = "child ping";
            Assert.Equal(payload, (await PingChild(payload)).Payload);
        }

        [Fact]
        public void PersistenceHubOnChildCreation()
        {
            //When_Hub_creates_a_child()
            When_hub_creates_a_child();
            //Time_to_life_is_set_and_can_differs_by_check_time()
            Assert.True(Child.ExpiresAt - BusinessDateTime.UtcNow >= Hub.ChildMaxInactiveTime - Hub.ChildClearPeriod);
            //Hub_should_contains_child()
            Assert.True(Hub.Children.ContainsKey(Infrastructure.ChildId));
            //Time_to_life_is_limited()
            Assert.True(Child.ExpiresAt - BusinessDateTime.UtcNow <= Hub.ChildMaxInactiveTime);
        }

        [Fact]
        public void When_child_became_inactive_too_long()
        {
            When_hub_creates_a_child();
            var actorToWatch = Child.Ref;
            Watch(actorToWatch);
            And_it_is_not_active_until_lifetime_period_is_expired();
            //It_should_be_removed_from_hub()
            Assert.False(Hub.Children.ContainsKey(Infrastructure.ChildId));
            //It_should_be_terminated()
            FishForMessage<Terminated>(m => m.ActorRef.Path == actorToWatch.Path);
        }

        [Fact]
        public async Task When_child_revives()
        {
            When_hub_creates_a_child();
            And_it_is_not_active_until_lifetime_period_is_expired();
            And_command_for_child_is_sent();
            //It_should_be_restored_after_command_execution()
            var payload = "child ping";
            Assert.Equal(payload, (await PingChild(payload)).Payload);

            //Hub_should_contains_child()
            Assert.True(Hub.Children.ContainsKey(Infrastructure.ChildId));
        }

    }
}