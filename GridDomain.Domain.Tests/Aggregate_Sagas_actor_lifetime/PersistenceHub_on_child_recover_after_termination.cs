using NUnit.Framework;

namespace GridDomain.Tests.Aggregate_Sagas_actor_lifetime
{

    [TestFixture(PersistentHubTestsStatus.PersistenceCase.Saga)]
    [TestFixture(PersistentHubTestsStatus.PersistenceCase.Aggregate)]
    class PersistenceHub_on_child_recover_after_termination : PersistentHub_childs_lifetime_test
    {

        [SetUp]
        public void When_child_became_inactive_too_long()
        {
            When_hub_creates_a_child();
            And_it_is_not_active_until_lifetime_period_is_expired();
            And_command_for_child_is_sent();
        }

        [Then]
        public void It_should_be_restored_after_command_execution()
        {
            Assert.True(PersistentHubTestsStatus.ChildExistence.Contains(_aggregateId));
        }

        [Then]
        public void It_should_be_watched_in_hub()
        {
            Assert.True(PersistentHubTestsStatus.ChildTerminationTimes.ContainsKey(_aggregateId));
        }

        public PersistenceHub_on_child_recover_after_termination(PersistentHubTestsStatus.PersistenceCase @case) : base(@case)
        {
        }
    }
}