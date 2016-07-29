using NUnit.Framework;

namespace GridDomain.Tests.Aggregate_Sagas_actor_lifetime
{

    [TestFixture(PersistentHubTestsStatus.PersistenceCase.Saga)]
    [TestFixture(PersistentHubTestsStatus.PersistenceCase.Aggregate)]
    class PersistenceHub_on_child_inactive : PersistentHub_childs_lifetime_test
    {
        [SetUp]
        public void When_child_became_inactive_too_long()
        {
            When_hub_creates_a_child();
            And_it_is_not_active_until_lifetime_period_is_expired();
        }

        [Then]
        public void It_should_be_removed_from_hub()
        {
            Assert.False(PersistentHubTestsStatus.ChildTerminationTimes.ContainsKey(_aggregateId));
        }

        [Then]
        public void It_should_be_terminated()
        {
            Assert.False(PersistentHubTestsStatus.ChildExistence.Contains(_aggregateId));
        }

        public PersistenceHub_on_child_inactive(PersistentHubTestsStatus.PersistenceCase @case) : base(@case)
        {
        }
    }
}