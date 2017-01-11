using Akka.Actor;
using NUnit.Framework;

namespace GridDomain.Tests.Unit.Aggregate_Sagas_actor_lifetime
{

    [TestFixture(PersistentHubTestsStatus.PersistenceCase.IstanceSaga)]
    [TestFixture(PersistentHubTestsStatus.PersistenceCase.Aggregate)]
    class PersistenceHubOnChildInactive : PersistentHub_children_lifetime_test
    {
        private IActorRef _actorToWatch;

        [SetUp]
        public void When_child_became_inactive_too_long()
        {
            When_hub_creates_a_child();
            _actorToWatch = Child.Ref;
            Watch(_actorToWatch);
            And_it_is_not_active_until_lifetime_period_is_expired();
        }

        [Then]
        public void It_should_be_removed_from_hub()
        {
            Assert.False(Hub.Children.ContainsKey(Infrastructure.ChildId));
        }

        [Then]
        public void It_should_be_terminated()
        {
            ExpectTerminated(_actorToWatch);
        }

        public PersistenceHubOnChildInactive(PersistentHubTestsStatus.PersistenceCase @case) : base(@case)
        {
        }
    }
}