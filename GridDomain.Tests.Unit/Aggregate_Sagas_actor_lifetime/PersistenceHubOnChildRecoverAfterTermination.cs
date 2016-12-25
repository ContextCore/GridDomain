using NUnit.Framework;

namespace GridDomain.Tests.Unit.Aggregate_Sagas_actor_lifetime
{

    [TestFixture(PersistentHubTestsStatus.PersistenceCase.IstanceSaga)]
    [TestFixture(PersistentHubTestsStatus.PersistenceCase.Aggregate)]
    [TestFixture(PersistentHubTestsStatus.PersistenceCase.StateSaga)]
    class PersistenceHubOnChildRecoverAfterTermination : PersistentHub_children_lifetime_test
    {

        [SetUp]
        public void When_child_revives()
        {
            When_hub_creates_a_child();
            And_it_is_not_active_until_lifetime_period_is_expired();
            And_command_for_child_is_sent();
        }

        [Then]
        public void It_should_be_restored_after_command_execution()
        {
            var payload = "child ping";
            Assert.AreEqual(payload, PingChild(payload).Payload);
        }

        [Then]
        public void Hub_should_contains_child()
        {
            Assert.True(Hub.Children.ContainsKey(Infrastructure.ChildId));
        }

        public PersistenceHubOnChildRecoverAfterTermination(PersistentHubTestsStatus.PersistenceCase @case) : base(@case)
        {
        }
    }
}